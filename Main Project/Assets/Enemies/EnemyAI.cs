using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Pool;
public enum PhaseChanges
{
	Confirmation,
 	FirstStrike,
  	Engagement,
   	Reinforcement,
    FinalAttack,
    Neutralization,
    Retreat
}

public class EnemyAI : MonoBehaviour
{
    public Transform muzzlePoint;
    public Transform player;
    public float range = 100f;
    public float fireRate = 0.5f;
    public float horizontalSpread = 0.05f;
    public float verticalSpread = 0.05f;
    public TrailRenderer trailPrefab;
    public LayerMask playerMask;
    public int enemyDamage;
    public int enemyBulletPower;
    public float bulletPen;

    private float nextFireTime;
    private ObjectPool<TrailRenderer> trailPool;

    private void Start()
    {
        // Create the trail object pool
        trailPool = new ObjectPool<TrailRenderer>(
            CreateTrail,
            trail =>
            {
                trail.gameObject.SetActive(true);
                trail.emitting = false;
                trail.Clear(); // Clear previous trail data
            },
            trail =>
            {
                trail.gameObject.SetActive(false);
            },
            trail =>
            {
                Destroy(trail.gameObject);
            },
            false,  // collectionCheck
            10,     // defaultCapacity
            50      // maxSize
        );
    }

    private TrailRenderer CreateTrail()
    {
        TrailRenderer trail = Instantiate(trailPrefab);
        trail.gameObject.SetActive(false);
        return trail;
    }

    private void Update()
    {
        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void Shoot()
    {
        // Calculate spread
        float spreadX = Random.Range(-horizontalSpread, horizontalSpread);
        float spreadY = Random.Range(-verticalSpread, verticalSpread);

        // Direction from muzzle to player with spread
        Vector3 direction = (player.position - muzzlePoint.position).normalized;
        direction += new Vector3(spreadX, spreadY, 0);

        if (Physics.Raycast(muzzlePoint.position, direction, out RaycastHit hit, range, playerMask))
        {

            // Spawn trail
            TrailRenderer trail = trailPool.Get();
            trail.transform.position = muzzlePoint.position;
            trail.emitting = true;

            StartCoroutine(PlayTrail(trail, muzzlePoint.position, hit.point));
        }
        else
        {
            // Missed hit, send trail to max range
            Vector3 endPos = muzzlePoint.position + direction.normalized * range;
            TrailRenderer trail = trailPool.Get();
            trail.transform.position = muzzlePoint.position;
            trail.emitting = true;

            StartCoroutine(PlayTrail(trail, muzzlePoint.position, endPos));
        }
    }

    private System.Collections.IEnumerator PlayTrail(TrailRenderer trail, Vector3 start, Vector3 end)
    {
        float time = 0;
        float duration = 0.1f;
        while (time < duration)
        {
            trail.transform.position = Vector3.Lerp(start, end, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        trail.transform.position = end;

        yield return new WaitForSeconds(trail.time);
        trailPool.Release(trail);
    }
}

/*
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Pool;
public enum PhaseChanges
{
	Confirmation,
 	FirstStrike,
  	Engagement,
   	Reinforcement,
    FinalAttack,
    Neutralization,
    Retreat
}
public class EnemyAI : MonoBehaviour
{
	private EnemyAnimationHandler animHandler;
	
    //Potential MultiPoint Patrol System Using int pointDirection instead of bool pointDirection
	public NavMeshAgent agent;
    public Transform player;

    public float armorQuality, bulletPen;
    public int enemyBulletPower;
    private int health;
    public int maxHealth,  enemyDamage, armorPoints, enemyStoppingPower, enemyXP;

    public LayerMask groundMask, playerMask;

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
	
	public Transform muzzle;
    public Transform target;
    public float hSpread = 5f;
    public float vSpread = 5f;

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
        Vector3 direction = player.position - transform.position;
		direction.y = 0f; // Ignore vertical difference
		Quaternion lookRotation = Quaternion.LookRotation(direction);
		transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 200f);
		
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
        Vector3 aimPoint = player.position + Vector3.up * 1.5f;
        Vector3 baseDir = (aimPoint - transform.position).normalized;

        Quaternion spread = Quaternion.Euler(
            Random.Range(-verticalSpread, verticalSpread),
            Random.Range(-horizontalSpread, horizontalSpread),
            0f
        );
        Vector3 direction = spread * baseDir;

	

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
					playerStats.damagePlayer(enemyDamage, bulletPen, enemyBulletPower);
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
	void shoot()
    {
        Vector3 aimPoint = target.position + Vector3.up;
        Vector3 baseDir = (aimPoint - muzzle.position).normalized;

        Quaternion spread = Quaternion.Euler(
            Random.Range(-vSpread, vSpread),
            Random.Range(-hSpread, hSpread),
            0f
        );
        Vector3 finalDir = (player.position - muzzle.position).normalized;

        Debug.Log("Direction: " + finalDir);

        if (Physics.Raycast(muzzle.position, finalDir, out RaycastHit hit, range))
        {
            Debug.Log("Hit " + hit.collider.name);
        }
		if (bulletsLeft > 0)
        {
            StartCoroutine(PlayTrail(muzzle.position, rayHit.point, rayHit));
        }
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
		
		animHandler = GetComponentInChildren<EnemyAnimationHandler>();

		if (animHandler == null)
		{
			Debug.LogError("EnemyAnimationHandler not found in children of " + gameObject.name);
		}
		
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
		shoot();
		if (!playerInAttackRange && playerInLineOfSight)
		{
			chasing();
			// animHandler.guardIsRunning();
		}

		else if (playerInAttackRange && playerInLineOfSight)
		{
			
			if (HasClearShot())
			{
				// animHandler.guardIsShooting();
				attacking();
			}
			else if (!HasClearShot()) 
			{
				chasing();
				// animHandler.guardIsRunning();
			}
            
        }
		
    }

    public void takeDamage(int damageTaken, float armorPen, int bulletPower)
    {
        float finalDamageTaken = 0;
        if (bulletPower <= enemyStoppingPower && armorPoints > 0)
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
            player.GetComponent<PlayerStats>().gainExperience(enemyXP);
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
*/