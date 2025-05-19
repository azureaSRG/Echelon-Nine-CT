using UnityEngine;

public class DeathBarriorScript : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Death Barrier"))
        {
            Debug.Log("The Player Has Hit the Barrier of Immense Pain & Agony.");

            playerStats.alive = false;
        }
    }
}
