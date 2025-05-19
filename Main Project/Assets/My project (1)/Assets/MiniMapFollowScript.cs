/*
 This program was made using the help of ChatGPT, though the original was not. I am only using it to help me program because I was taking too long to code and I am not too proficent in C++
Though learning.
 */
// This defines the purpose of the script, similar to <DOC TYPE HTML>
using UnityEngine;


public class MiniMapFollowScript : MonoBehaviour
{
    // defines player as object
    public Transform player;

    // This is the cameras default height
    public float height = 50f;

    // T
    public bool rotateWithPlayer = true;
    // Updates the position after 
    void LateUpdate()
    {
        if (player == null) return;

        Vector3 newPosition = player.position;
        newPosition.y = player.position.y + height;
        transform.position = newPosition;

        if (rotateWithPlayer)
        {
            transform.rotation = Quaternion.Euler(90f, player.eulerAngles.y, 0f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        }
    }
}