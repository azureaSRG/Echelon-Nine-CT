using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Pool;

public class EnemyAI : MonoBehaviour
{
	private GuardAnimationHandler animHandler;
	
    //Potential MultiPoint Patrol System Using int pointDirection instead of bool pointDirection
	public NavMeshAgent agent;
    public Transform player;

    public float armorQuality, bulletPen;
    public int guardBulletPower;
    private int health;
    public int maxHealth,  guardDamage, armorPoints, guardStoppingPower, guardXP;

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
                    rayHit.collider.GetComponent<PlayerStats>().damagePlayer(guardDamage, bulletPen, guardBulletPower);
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
		if (!playerInAttackRange && playerInLineOfSight)
		{
			chasing();
			animHandler.guardIsRunning();
		}

		else if (playerInAttackRange && playerInLineOfSight)
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
}
