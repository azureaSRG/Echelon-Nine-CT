using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum Actions
{
	Idle,
	SlowWalk,
	Walking,
	Running,
	Shooting
}

public class PlayerMovement : MonoBehaviour
{

    public Transform Camera;

    public CharacterController controller;
    public float baseSpeed = 12f;
    private float speed;
    public float gravity = -9.81f;
    
    private float maxStamina = 100f;
    private float playerStamina;
    
    public Transform groundCheck;
    public Transform airCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
	public LayerMask enemyMask;

    Vector3 velocity;
    bool isGrounded;
    bool inAir;
	
	public float noiseLevel;
	public Actions action;
	
	void Awake()
	{
		action = Actions.Idle;
	}
	
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerStamina = maxStamina;
    }

    // Update is called once per frame
    void Update()
    {
		
		//Avoid repeated use if redudant
		if (GuardAI.phase != 4)
		{
		_noiseManager(getInput());
		}
		
        //Sphere under player checks if the player is in contact with the ground
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        inAir = Physics.CheckSphere(airCheck.position, groundDistance, groundMask);
		
        //Prevents gravity from building up over time
        if (isGrounded && velocity.y <0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        
        Vector3 forward = Camera.transform.forward;
        Vector3 right = Camera.transform.right;
        
        //Prevents movement from locking when z -> 0 by returning a magnitude of 1; assisted by clamped camera rotation.
        forward.y = 0; // Remove the vertical component in the forward direction
        forward.Normalize(); // Ensure the vectors have a value of 1. The value is required to be 1, as a decimal or zero, would slow down or stop the player from moving. This allows the player to move in the same direction with different magnitude.

        
        //Changes player speed depending on the key pressed or aiming
        if (Input.GetKey(KeyCode.LeftShift) && playerStamina > 0 && !Input.GetButton("Fire2")) 
        {
            speed = baseSpeed * 1.5f;
            playerStamina -= 10*Time.deltaTime ;
        }
        
        else if (Input.GetKey(KeyCode.LeftControl)) 
        {
            speed = baseSpeed * 0.5f;
        }
        else
        {
            speed = baseSpeed;
        }

        if (Input.GetButton("Fire2"))
        {
            speed = speed / 3;
        }
        
        //Adds player stamina if left shift isn't held down
        if (playerStamina < maxStamina && !Input.GetKey(KeyCode.LeftShift))
        {
            playerStamina += 2 * Time.deltaTime;
        }

        //Continues moving player even if in air
        Vector3 move = right * x + forward * z;
        controller.Move(move * speed * Time.deltaTime);

        //Debug.Log(Camera.transform.forward);
        //Debug.Log(velocity.y);

        //Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
		
		

    }
	
	private string getInput()
	{
		if (Input.GetAxis("Horizontal") > 0 || Input.GetAxis("Vertical") > 0)
		{
			if (Input.GetKey(KeyCode.LeftShift) && playerStamina > 0 && !Input.GetButton("Fire2")) 
			{
				return "Running";
			}
			
			else if (Input.GetKey(KeyCode.LeftControl)) 
			{
				return "Slow Walking";
			}
			else
			{
				return "Walking";
			}
		}	
		else
		{
			return "Idle";
		}
	}
	
	private void OnDrawGizmos()
	{
		Gizmos.color = new Color(1,0,0,0.3f);
		Gizmos.DrawSphere(transform.position, noiseLevel);
		
		Handles.color = new Color(1,0,0,0.3f);
		Handles.DrawSolidDisc(transform.position, transform.up, noiseLevel);
	}
	//GetComponent<PlayerStats>().damagePlayer(guardDamage, bulletPen, guardBulletPower);
	private void _noiseManager(string playerAction)
	{	
		switch (action)
		{
			//It should be impossible for guards to detect idle players from noise alone
			case Actions.Idle:
			{
				noiseLevel = 40 + Random.Range(-3f,3f);
				//Alert Level does not change
				if (playerAction == "Walking")
				{
					action = Actions.Walking;
				}
				else if (playerAction == "Slow Walking")
				{
					action = Actions.SlowWalk;
				}
				else if (playerAction == "Running")
				{
					action = Actions.Running;
				}
				break;
			}
			
			//Slow walking should not detect guards from noise alone
			case Actions.SlowWalk:
			{
				noiseLevel = 50 + Random.Range(-5f,5f);
				if (playerAction == "Walking")
				{
					action = Actions.Walking;
				}
				else if (playerAction == "Running")
				{
					action = Actions.Running;
				}
				else if (playerAction == "Idle")
				{
					action = Actions.Idle;
				}
				break;
			}
			
			//Walking causes guard to detect by noise
			case Actions.Walking:
			{
				noiseLevel = 60 + Random.Range(-9f,9f);
				if (checkGuardsInNoiseRadius(noiseLevel))
				{
					increaseAlertLevelOnGuards(1,noiseLevel);
				}
				
				if (playerAction == "Slow Walking")
				{
					action = Actions.SlowWalk;
				}
				else if (playerAction == "Running")
				{
					action = Actions.Running;
				}
				else if (playerAction == "Idle")
				{
					action = Actions.Idle;
				}
				break;
			}
			
			case Actions.Running:
			{
				noiseLevel = 75 + Random.Range(-12f,12f);
				if (checkGuardsInNoiseRadius(noiseLevel))
				{
					increaseAlertLevelOnGuards(1,noiseLevel);
				}
				
				if (playerAction == "Slow Walking")
				{
					action = Actions.SlowWalk;
				}
				else if (playerAction == "Walking")
				{
					action = Actions.Walking;
				}
				else if (playerAction == "Idle")
				{
					action = Actions.Idle;
				}
				break;
			}
		}
	}
	
	private void increaseAlertLevelOnGuards(int alertIncrease, float noiseAmount)
	{
		Collider[] guardsInRadius = Physics.OverlapSphere(transform.position, noiseAmount);
		foreach (Collider c in guardsInRadius)
		{
			if (c.CompareTag("Guard"))
			{
				c.GetComponent<GuardAI>().changeAlertLevel(alertIncrease);
				Debug.Log(c);
			}
		}
	}
	
	private bool checkGuardsInNoiseRadius(float noiseAmount)
	{
		Collider[] checkRadius = Physics.OverlapSphere(transform.position, noiseAmount);
		foreach (Collider c in checkRadius)
		{
			if (c.CompareTag("Guard"))
			{
				return c.CompareTag("Guard");
			}
		}
		return false;
	}
	
}
