using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public Transform Camera;

    public CharacterController controller;

    public float speed = 12f;
    public float gravity = -9.81f;

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

        //Prevents inputs from being retrived if not on ground 
        if (inAir)
        {
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
        }
        
        Vector3 forward = Camera.transform.forward;
        Vector3 right = Camera.transform.right;
        
        //Prevents movement from locking when z -> 0 by returning a magnitude of 1; assisted by clamped camera rotation.
        forward.y = 0; // Remove the vertical component in the forward direction
        forward.Normalize(); // Ensure the vectors have a value of 1. The value is required to be 1, as a decimal or zero, would slow down or stop the player from moving. This allows the player to move in the same direction with different magnitude.

        
        //Changes player speed depending on the key pressed
        if (Input.GetKey(KeyCode.LeftShift)) {speed = 15f;}
        else if (Input.GetKey(KeyCode.LeftControl)) {speed = 9f;}
        else {speed = 12f;}
        
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
