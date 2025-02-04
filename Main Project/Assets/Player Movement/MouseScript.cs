using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseScript : MonoBehaviour
{

    public float mouseSensitivity;
    private float mouseSpeed;

    public Transform playerBody;

    float xRotation = 0f;
    float yRotation = 0f;

    private findMouseSpeed(int rate)
    {
        speed = mouseSensitivity/rate;
        return speed;
    }
    // Start is called before the first frame update
    void Start()
    {
        //Locks the cursor to the middle and makes it invisible
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Fire2"))
        {
            mouseSpeed = findMouseSpeed(4);
        }
        else
        {
            mouseSpeed = findMouseSpeed(1);
        }

        //Mouse Inputs
        float mouseX = Input.GetAxis("Mouse X") * mouseSpeed * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSpeed * Time.deltaTime;

        //Rotates along axis specified (Ex: on x skewer)
        xRotation -= mouseY;

        //Limits/Clamps rotation
        xRotation = Mathf.Clamp(xRotation, -89f, 89f);

        yRotation += mouseX;

        //Applies rotation
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
        

    }
}
