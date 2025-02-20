using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Pool;

public class GuardAI : MonoBehaviour
{

    //Potential MultiPoint Patrol System Using int pointDirection instead of bool pointDirection
    public NavMeshAgent agent;
    public Transform player;

    public Transform startingPatrolPoint;
    public Transform endingPatrolPoint;

    public Vector3 vectorStart;
    public Vector3 vectorEnd;
    private bool pointDirection = false;
    public bool stationary = false;
    public bool isWandering = false;

    public float armorQuality;
    private int health;
    public int maxHealth,  guardDamage, armorPoints;

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

    //Enemy Shooting
    public float horizontalSpread, verticalSpread;
    public RaycastHit rayHit;
    public int range;
    public int magCapacity;
    public float reloadTime;

    private bool reloading;
    [SerializeField]
    private int bulletsLeft, bulletsShot;


    //Trails
    public Material Material;
    public AnimationCurve WidthCurve;
    public float Duration = 0.5f;
    public float MinVertexDistance = 0.1f;
    public Gradient Color;
    //public ImpactType ImpactType;

    public float MissDistance = 100f;
    public float SimulationSpeed = 200f;
    private ObjectPool<TrailRenderer> TrailPool;
    public Transform TrailLeave;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        vectorStart = new Vector3(startingPatrolPoint.position.x, startingPatrolPoint.position.y, startingPatrolPoint.position.z);
        vectorEnd = new Vector3(endingPatrolPoint.position.x, endingPatrolPoint.position.y, endingPatrolPoint.position.z);
        walkPointSet = true;
    }

    //PATROLLING STATE
    private void switchPointDirection()
    {
        if (!pointDirection)
        {
            agent.SetDestination(vectorEnd);
            if (Mathf.Abs((transform.position.x - endingPatrolPoint.position.x) + (transform.position.z - endingPatrolPoint.position.z)) < 1)
            {
                pointDirection = true;
            }
        }
        else
        {
            agent.SetDestination(vectorStart);
            if (Mathf.Abs((transform.position.x - startingPatrolPoint.position.x) + (transform.position.z - startingPatrolPoint.position.z)) < 1)
            {
                pointDirection = false;
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

    private void patrolling()
    {
        // There are two cases: Enemy wonders randomly or between two points
        // Enemy wanders between two predetermined points
        if (!isWandering)
        {
            Invoke("switchPointDirection", 0f);
        }

        if (playerInSightRange)
        {
            Debug.LogWarning("Patrolling function is still being called while player is in sight!");
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

    //CHASING STATE
    private void chasing()
    {

        NavMeshPath path = new NavMeshPath();
        if (agent.CalculatePath(player.position, path))
        {
            agent.SetPath(path);
        }

    }

    //ATTACK STATE
    private void attacking()
    {
        agent.SetDestination(transform.position);
        transform.LookAt(player);

        if (!alreadyShot)
        {
            shoot();
        }
        
    }

    //SHOOTING
    private void shoot()
    {
        alreadyShot = true;

        //Spread
        float spreadX = Random.Range(-horizontalSpread, horizontalSpread);
        float spreadY = Random.Range(-verticalSpread, verticalSpread);

        Vector3 direction = transform.forward + new Vector3(spreadX, spreadY, 0);

        if (bulletsLeft > 0)
        {
            StartCoroutine(PlayTrail(TrailLeave.transform.position, rayHit.point, rayHit));
        }
        

        if (Physics.Raycast(transform.position, direction, out rayHit, range, playerMask))
        {
            if (rayHit.collider.CompareTag("Player"))
                {
                    rayHit.collider.GetComponent<PlayerStats>().damagePlayer(guardDamage);
                }
        }

        Invoke("resetShooting", timeBetweenShots);

        if (bulletsShot > 0 && bulletsLeft > 0)
        {
            //Executes the shoot function and has cooldown of firerate (TBS)
            Invoke("shoot", timeBetweenShots);
        }
        else if (bulletsLeft < 0 && !reloading)
        {
            Reload();
        }

        bulletsLeft--;
    }

    private void resetShooting()
    {
        alreadyShot = false;
    }

    // RELOADING
    private void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        bulletsLeft = magCapacity;
        reloading = false;
    }

    // TRAILS
    private TrailRenderer CreateTrail()
    {
        GameObject instance = new GameObject("Bullet Trail");
        TrailRenderer trail = instance.AddComponent<TrailRenderer>();
        trail.colorGradient = Color;
        trail.material = Material;
        trail.widthCurve = WidthCurve;
        trail.minVertexDistance = MinVertexDistance;

        trail.emitting = false;
        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        return trail;
    }

    //BULLET TRAILS
    private IEnumerator PlayTrail(Vector3 StartPoint, Vector3 EndPoint, RaycastHit Hit)
    {
        TrailRenderer instance = TrailPool.Get();
        instance.gameObject.SetActive(true);
        instance.transform.position = StartPoint;
        instance.Clear();
        yield return null;

        instance.emitting = true;

        float distance = Vector3.Distance(StartPoint, EndPoint);
        float remainingDistance = distance;
        while (remainingDistance > 0f)
        {
            instance.transform.position = Vector3.Lerp(StartPoint, EndPoint, Mathf.Clamp01(1 - (remainingDistance / distance)));
            remainingDistance -= SimulationSpeed * Time.deltaTime;
            yield return null;
        }

        instance.transform.position = EndPoint;

        if (Hit.collider != null)
        {
            //SurfaceManager.Instance.HandleImpact(Hit.transform.gameObject, EndPoint, Hit.normal, ImpactType, 0);
        }

        yield return new WaitForSeconds(Duration);
        yield return null;
        instance.emitting = false;
        instance.gameObject.SetActive(false);
        TrailPool.Release(instance);
    }


    private void Start()
    {
        bulletsLeft = magCapacity;
        health = maxHealth;

        TrailPool = new ObjectPool<TrailRenderer>(
        CreateTrail,
        trail =>
        {
            trail.gameObject.SetActive(true);
            trail.emitting = false;
        },
        trail =>
        {
            trail.gameObject.SetActive(false);
        },
        trail =>
        {
            Destroy(trail.gameObject);
        },
        false, // collectionCheck
        10,    // defaultCapacity
        50     // maxSize
    );
    }

    private void Update()
    {

        //Check Sight
        Debug.Log(agent.hasPath);

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

        else if (playerInAttackRange && playerInSightRange)
        {
            
            if (HasClearShot())
            {
                attacking();
            }
            else if (!HasClearShot()) 
            {
                chasing();

            }
            
        }
    }

    public void takeDamage(int damageTaken, float armorPen)
    {
        float finalDamageTaken = 0;
        if (armorQuality >= armorPen)
        {
            finalDamageTaken = damageTaken*(1-armorQuality);
            armorPoints--;
        }
        else if (armorQuality < armorPen)
        {
            finalDamageTaken = damageTaken;
        }

        health -= Mathf.RoundToInt(finalDamageTaken);

        if (health < 0)
        {
            Destroy(gameObject);
        }
    }

    private bool HasClearShot()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, attackRange))
        {
            return hit.collider.CompareTag("Player"); // Ensure it actually hits the player
            
        }
        return false;
    }
}
