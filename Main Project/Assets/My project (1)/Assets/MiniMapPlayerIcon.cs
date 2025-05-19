// imports unity namespace
using UnityEngine;

// imports unityUI namespace
using UnityEngine.UI;

/*
the word public is used to tell that the script is accessible by other scripts, class says that the characters after this word is the name of the program, and “: Monobehaviour” gives the script a base class/and attributes
*/
public class MinimapPlayerIcon : MonoBehaviour
{
    public Transform player; // provides input for player to specify what the code is to be acted towards

    public RectTransform mapImage;  // contains the static map

    public RectTransform playerMarker; // Reference to PlayerMarker UI Image

    /* input for map size, acts to know the range that the custom minimap range has over the actual map */
    public float mapScale = 10f;



    void Update()
    {
        if (player == null) return;

        // Keep the player marker in the center of the minimap
        playerMarker.anchoredPosition = Vector2.zero;

        // Rotate the player marker based on player rotation
        playerMarker.localRotation = Quaternion.Euler(0, 0, -player.eulerAngles.y);
    }
}





