using UnityEngine;

public class Phases : MonoBehaviour
{
    protected int playerNoiseLevel;
    public GameObject playerObject;
    public int currentPhase;
    public bool inStealth;
    public int enemyAwareness;
    public int difficultyLevel;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void checkPhase()
    {
        
    }
}
