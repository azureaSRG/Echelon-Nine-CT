using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerStats : MonoBehaviour
{

    [SerializeField]
    protected int health;
    [SerializeField]
    protected int maxHealth;
    [SerializeField]
    protected bool alive;

    public bool playerInStealth;
    public int phase = 0;
    /*
    Stealth Phases
    0: Stealth
    1: Suspicion
    2: Search
    3: Alert

    Engagement Phases
    0: Contact
    1: First Strike
    2: Engagement
    3: Reinforcements
    4: Final Attack
    ! 5 & 6 are exclusive events
    5: Neutralization
    6: Retreat
    */

    public void checkHealth()
    {
        if (health < 0)
        {
            alive = false;
            health = 0;
        }

        if (health > maxHealth)
        {
            health = maxHealth;
        }

    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
