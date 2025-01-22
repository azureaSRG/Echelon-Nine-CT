using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseScript : MonoBehaviour
{

    public float mouseSensitivity = 100f;

    public Transform playerBody;

    float xRotation = 0f;
    float yRotation = 0f;


    // Start is called before the first frame update
    void Start()
    {
        //Locks the cursor to the middle and makes it invisible
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        //Mouse Inputs
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

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
