using UnityEngine;
using UnityEngine.AI;

public class EnemyAIScrpit : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;

    public Transform startingPatrolPoint;
    public Transform endingPatrolPoint;

    public Vector3 vectorStart;
    public Vector3 vectorEnd;
    private bool pointDirection = false;
    public bool isWandering = false;

    public float maxhealth, armor;
    private float health;
    public float damage;

    public LayerMask groundMask, playerMask;

    //Patrolling
    public Vector3 walkPoint;
    bool walkPointSet;
    float walkPointRange = 1f;

    //Attacking
    public float timeBetweenShots;
    bool alreadyShot;

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        vectorStart = new Vector3(startingPatrolPoint.position.x, startingPatrolPoint.position.y, startingPatrolPoint.position.z);
        vectorEnd = new Vector3(endingPatrolPoint.position.x, endingPatrolPoint.position.y, endingPatrolPoint.position.z);
        walkPointSet = true;
    }

    private void patrolling()
    {
        // There are two cases: Enemy wonders randomly or between two points
        // Enemy wanders between two predetermined points
        if (!isWandering)
        {
            if (!pointDirection)
            {
                agent.SetDestination(vectorEnd);
                Vector3 distanceToWalkPoint = transform.position - endingPatrolPoint.position;
                if (Mathf.Abs((transform.position.x - endingPatrolPoint.position.x) + (transform.position.z - endingPatrolPoint.position.z)) < 1)
                {
                    pointDirection = true;
                }
            }
            else if (pointDirection)
            {
                agent.SetDestination(vectorStart);
                Vector3 distanceToWalkPoint = transform.position - startingPatrolPoint.position;
                if (Mathf.Abs((transform.position.x - startingPatrolPoint.position.x) + (transform.position.z - startingPatrolPoint.position.z)) < 1)
                {
                    pointDirection = false;
                }
            }
        }
        
        // Enemy wanders randomly
        else if (isWandering)
        {
            if (!walkPointSet)
            {
                searchWalkPoint();
            }

            if (walkPointSet)
            {
                agent.SetDestination(walkPoint);
            }

            Vector3 distanceToWalkPoint = transform.position - walkPoint;

            if (distanceToWalkPoint.magnitude < 1f)
            {
                walkPointSet = false;
            }
        }
    }
    
    //Enemy searchs for random point to walk
    private void searchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        //Checks if in bounds
        if (Physics.Raycast(walkPoint, -transform.up, 2f, groundMask))
        {
            walkPointSet = true;
        }
    }

    //Chases player
    private void chasing()
    {
        agent.SetDestination(player.position);
    }

    //Attacks player
    private void attacking()
    {
        agent.SetDestination(transform.position);
        transform.LookAt(player);

        if (!alreadyShot)
        {
            //Attacking Code
            //ATTACKING CODE HERE
            //End

            alreadyShot = true;
            Invoke(nameof(resetShooting), timeBetweenShots);
        }
    }

    private void resetShooting()
    {
        alreadyShot = false;
    }

    private void Update()
    {
        //Check Sight
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerMask);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerMask);

        //Controls Enemy Behavior
        if (!playerInSightRange && !playerInAttackRange)
        {
            patrolling();
        }

        else if (!playerInAttackRange && playerInSightRange)
        {
            chasing();
        }

        else
        {
            attacking();
        }

    }

    public void takeDamage()
    {
        health -= damage;

        if (health < 0)
        {
            Destroy(gameObject);
        }
    }

    private void testing()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
