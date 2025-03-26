using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Pool;
using UnityEditor;

public enum AlertStage
{
	Stealth,
	Suspicion,
	Search,
	Alert
}

public class GuardAI : MonoBehaviour
{
	private GuardAnimationHandler animHandler;
	
    //Potential MultiPoint Patrol System Using int pointDirection instead of bool pointDirection
	public static int phase = 0;
	public NavMeshAgent agent;
    public Transform player;

    public Transform startingPatrolPoint;
    public Transform endingPatrolPoint;

    public Vector3 vectorStart;
    public Vector3 vectorEnd;
    private bool pointDirection = false;
    public bool stationary = false;
    public bool isWandering = false;

    public float armorQuality, bulletPen;
    public int guardBulletPower;
    private int health;
    public int maxHealth,  guardDamage, armorPoints, guardStoppingPower, guardXP;

    public LayerMask groundMask, playerMask;

    //Patrolling
    public Vector3 walkPoint;
    bool walkPointSet;
    float walkPointRange = 10f;

    //Attacking
    public float timeBetweenShots;
    bool alreadyShot;

    //States
    public float attackRange;
    public bool playerInAttackRange, playerInLineOfSight;

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
    float prevDetectionRadius,prevDetectionAngle;
    public float detectionRadius;
    public float detectionAngle;
    public float innerDetectionRadius;
    public float innerDetectionAngle;
    public AlertStage alertStage;	
    
    [Range (0,200)] public float alertLevel = 0;
    [Range(0,600)] public int searchTimer = 300;
	
    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
		
        vectorStart = new Vector3(startingPatrolPoint.position.x, startingPatrolPoint.position.y, startingPatrolPoint.position.z);
        vectorEnd = new Vector3(endingPatrolPoint.position.x, endingPatrolPoint.position.y, endingPatrolPoint.position.z);
        walkPointSet = false;
    }

    //PATROLLING STATE
    private void switchPointDirection()
	{
		if (transform.position.x == startingPatrolPoint.position.x)
		{
			pointDirection = true;
		}
		else if (transform.position.x == endingPatrolPoint.position.x)
		{
			pointDirection = false;
		}
		
		if (pointDirection)
		{
			agent.SetDestination(endingPatrolPoint.position);
		}
		else
		{
			agent.SetDestination(startingPatrolPoint.position);
		}
	}

    //Enemy searchs for random point to walk
    private void searchWalkPoint()
{
    float randomZ = Random.Range(-walkPointRange, walkPointRange);
    float randomX = Random.Range(-walkPointRange, walkPointRange);

    walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
    
    // Ensure NavMeshAgent can reach the point
    NavMeshHit hit;
    if (NavMesh.SamplePosition(walkPoint, out hit, 2.0f, NavMesh.AllAreas))
    {
        walkPoint = hit.position; // Adjust point to nearest valid NavMesh location
        walkPointSet = true;
        Debug.Log("Valid Walk Point Found: " + walkPoint);
    }
    else
    {
        Debug.Log("Invalid Walk Point: " + walkPoint);
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

        Vector3 direction = (player.position - transform.position).normalized + new Vector3(spreadX, spreadY, 0);

        if (bulletsLeft > 0)
        {
            StartCoroutine(PlayTrail(TrailLeave.transform.position, rayHit.point, rayHit));
        }
        

        if (Physics.Raycast(transform.position, direction, out rayHit, range, playerMask))
		{
			if (rayHit.collider.CompareTag("Player"))
			{
				PlayerStats playerStats = rayHit.collider.GetComponent<PlayerStats>();
				if (playerStats != null)
				{
					playerStats.damagePlayer(guardDamage, bulletPen, guardBulletPower);
				}
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
		
		animHandler = GetComponentInChildren<GuardAnimationHandler>();
			
		if (animHandler == null)
		{
			Debug.LogError("GuardAnimationHandler not found in children of " + gameObject.name);
		}
		
		prevDetectionRadius = detectionRadius;
		prevDetectionAngle = detectionAngle;
		alertStage = AlertStage.Stealth;
		
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
		Collider[] targetsInFOV = Physics.OverlapSphere(transform.position, detectionRadius);
		Collider[] targetsInInnerFOV = Physics.OverlapSphere(transform.position, innerDetectionRadius);
		playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerMask);
		playerInLineOfSight = CheckLineOfSight();
		
		bool playerDetected = false;
		foreach (Collider c in targetsInFOV)
		{
			if (c.CompareTag("Player"))
			{
				float signedAngle = Vector3.Angle(transform.forward,c.transform.position - transform.position);
				if (Mathf.Abs(signedAngle) < detectionAngle / 2)
				{
					playerDetected = true;
				}
				break;
			}
		}
 	
		foreach (Collider c in targetsInInnerFOV)
		{
			if (c.CompareTag("Player"))
			{
				float signedAngle = Vector3.Angle(transform.forward,c.transform.position - transform.position);
				if (Mathf.Abs(signedAngle) < innerDetectionAngle / 2)
				{
					playerDetected = true;
				}
			break;
			}
		}
		
		_UpdateAlertState(playerDetected);
	
        //Controls Enemy Behavior
		if (!playerDetected && !playerInLineOfSight && !playerInAttackRange && !stationary && phase != 3)
		{
			patrolling();
			animHandler.guardIsWalking();
		}

		else if ((!playerInAttackRange && playerDetected && playerInLineOfSight) || (phase == 3 && !playerInAttackRange))
		{
			chasing();
			animHandler.guardIsRunning();
		}

		else if (playerInAttackRange && playerDetected && playerInLineOfSight)
		{
			
			if (HasClearShot())
			{
				animHandler.guardIsShooting();
				attacking();
			}
			else if (!HasClearShot()) 
			{
				chasing();
				animHandler.guardIsRunning();
			}
            
        }
		
    }

    public void takeDamage(int damageTaken, float armorPen, int bulletPower)
    {
        float finalDamageTaken = 0;
        if (bulletPower <= guardStoppingPower && armorPoints > 0)
        {
            finalDamageTaken = damageTaken*(1-armorQuality);
            armorPoints--;
        }
        else if (armorPoints > 0 && armorQuality > armorPen)
        {
            finalDamageTaken = damageTaken * (1 - armorQuality);
        }
        else
        {
            finalDamageTaken = damageTaken;
        }

        health -= Mathf.RoundToInt(finalDamageTaken);
		
        if (health < 0)
        {
			Destroy(gameObject);
            player.GetComponent<PlayerStats>().gainExperience(guardXP);
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
	
	private bool CheckLineOfSight()
	{
		if (!Physics.CheckSphere(transform.position, detectionRadius, playerMask) && !Physics.CheckSphere(transform.position, innerDetectionRadius, playerMask)) return false;
		
		RaycastHit hit;
		if (Physics.Raycast(transform.position, (player.position - transform.position).normalized, out hit, detectionRadius))
		{
			return hit.collider.CompareTag("Player");
		}
		else if (Physics.Raycast(transform.position, (player.position - transform.position).normalized, out hit, innerDetectionRadius))
		{
			return hit.collider.CompareTag("Player");
		}
		return false;
	}
	
	//Debugging
	private void OnDrawGizmos()
	{
		Handles.color = new Color(0,1,0,0.3f);
		Vector3 arcStartDirection = Quaternion.AngleAxis(-detectionAngle / 2f, transform.up) * transform.forward;
		Handles.DrawSolidArc(transform.position, transform.up, arcStartDirection, detectionAngle, detectionRadius);
		
		Vector3 innerArcStartDirection = Quaternion.AngleAxis(-innerDetectionAngle / 2f, transform.up) * transform.forward;
		Handles.DrawSolidArc(transform.position, transform.up, innerArcStartDirection, innerDetectionAngle, innerDetectionRadius);
	}
	
	//Accessed by Player Movement for noise levels
	public void changeAlertLevel(float changeAmount)
	{
		alertLevel = alertLevel + changeAmount;
	}
	
	private void _UpdateAlertState(bool playerDetected)
	{
		//Debug.Log(alertStage);
		
		switch (alertStage)
		{
			case AlertStage.Stealth:
				if (playerDetected && CheckLineOfSight())
				{
					alertLevel++;
					if (alertLevel >= 50)
					{
						alertStage = AlertStage.Suspicion;
						phase = 1;
					}
				}
				if (phase == 2)
				{
					alertStage = AlertStage.Search;
				}
				else if (phase == 3)
				{
					alertStage = AlertStage.Alert;
				}
				
				break;
			
			case AlertStage.Suspicion:
				if (playerDetected && CheckLineOfSight())
				{
					alertLevel++;
					detectionAngle = prevDetectionAngle + 20;
					detectionRadius = prevDetectionRadius + 10;
					
					if (alertLevel >= 100)
					{
						alertStage = AlertStage.Search;
						phase = 2;
					}
				}
				else if (!playerDetected || !CheckLineOfSight())
				{
					alertLevel--;
				}
				
				if (phase == 2)
				{
					alertStage = AlertStage.Search;
				}
				else if (phase == 3)
				{
					alertStage = AlertStage.Alert;
				}
				
				
				break;
			
			case AlertStage.Search:
				if (playerDetected && CheckLineOfSight())
				{
					alertLevel++;
					searchTimer++;
					if (alertLevel >= 200)
					{
						alertStage = AlertStage.Alert;
						phase = 3;
					}
					
					if (alertLevel <= 0 && searchTimer <= 0)
					{
						alertStage = AlertStage.Suspicion;
						phase = 2;
						searchTimer = 300;
						alertLevel = 20;
					}
					
				}
				else
				{
					searchTimer--;
					alertLevel--;
				}
				
				if (phase == 3)
				{
					alertStage = AlertStage.Alert;
				}
				break;
			
			case AlertStage.Alert:
			//Cheap Workaround TBH
				detectionAngle = 360;
				detectionRadius = 1000;
				break;
		}
	}
}
