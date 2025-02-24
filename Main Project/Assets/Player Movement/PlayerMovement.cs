using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public Transform Camera;

    public CharacterController controller;
    public float baseSpeed = 12f;
    private float speed;
    public float gravity = -9.81f;
    public ModularGunSystem mass;
    
    private float maxStamina = 100f;
    private float playerStamina;
    
    public Transform groundCheck;
    public Transform airCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;
    bool isGrounded;
    bool inAir;


    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerStamina = maxStamina;
    }

    // Update is called once per frame
    void Update()
    {

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
}
