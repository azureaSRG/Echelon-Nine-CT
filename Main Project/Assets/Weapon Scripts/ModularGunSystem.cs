using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ModularGunSystem : MonoBehaviour
{
    /*
    Displayed Information (This can be manipulated in the inspector)
    These variables vary from gun to gun
    */
    public bool selection;
    public int excludedClasses;
    /*
     * 0 = None
     * 1 = Alpha Only
     * 2 = Bravo Only
     * 3 = Delta Only
     * 4 = Alpha & Charlie Only
     * 5 = Bravo & Charlie Only
     * 6 = Bravo & Delta Only
     * 7 = Alpha & Bravo Only
     */

    //Ammo Information
    public string caliber;
    public int magazineSize;
    public int magazineReserves;
    public bool fullAutoAllowed;
    public bool fullAuto;

    //Magazine Information
    private int[] magList = new int[] {0};

    //Damage Information
    public int headDamage, bodyDamage, legDamage, armDamage, bulletPower;
    public float armorPenetration;

    //Firing Information
    public float timeBetweenShots, muzzleVelocity, effectiveRange, maxRange;

    //Gun Information
    public float cost, probabilityOfMalfunction;
    public int mass;

    //Handling Information
    public float fullReloadTime, partialReloadTime;

    //Accuracy Information
    public float horizontalSpread;
    public float verticalSpread;
	public float firingSpreadRate, maxFiringSpread;
	
	//Camera Recoil
	public float recoilCamSpeed, recoilCamRecoverySpeed;
	public Vector3 recoilCam;
	public Vector3 recoilAimCam;
	
    /*
    Private Variables:
    These variables are changed based on the usage
    */

    private float firingSpread, movementInaccuracy;
    private int magazinesLeft, bulletsLeft, bulletsShot, bulletsPerTap;
    private bool shooting, readyToShoot, reloading;

    //Malfunctions
    private bool isMalfunction;
    private int malfunctionType;
	
	//CAMERA AND TRAILS
	
    //References
    public Camera fpsCam;
	public GameObject camHolder;
    public Transform attackPoint;
    public LayerMask enemyDef;

    //Bullet Collision Detection
    public RaycastHit rayHit;


    //Trails
    public Material Material;
    public AnimationCurve WidthCurve;
    public float Duration = 0.5f;
    public float MinVertexDistance = 0.1f;
    public Gradient Color;
    public Transform TrailLeave;
    public float MissDistance = 100f;
    public float SimulationSpeed = 200f;
    private ObjectPool<TrailRenderer> TrailPool;

    //Casing Ejection
    public GameObject shellPrefab;
    public Transform shellEjectionPoint;

    //RELOADING
    //Refills ammo at beginning
    private void RefillAmmo()
    {
        magazinesLeft = magazineReserves;
        magList = new int[magazineReserves];
        for (int x = 0; x < magazineReserves; x++)
        {
            magList[x] = magazineSize;
        }
    }

    //Sets bullets left to the mag size then sets reloading to false
    private void ReloadFinished()
    {
        if (bulletsLeft == 0)
        {
            magazinesLeft--;
        }

        bulletsLeft = magazineSize;
        reloading = false;
    }

    //Sets reloading and delays reload finsihed function by time
    private void Reload()
    {
        reloading = true;
        if (bulletsLeft > 0)
        {
            Invoke("ReloadFinished", partialReloadTime);
        }
        else
        {
            Invoke("ReloadFinished", fullReloadTime);
        }

    }

    //Resets shooting variable
    private void ResetShooting()
    {
        readyToShoot = true;
    }
	
	//Firing Spread Variation and Movement Varitation
    private float findShootingVariation()
    {
		//Finds speed based on velocity of camera
        float movementVariation = fpsCam.velocity.magnitude / 10;
        float modifiedMaxFiringSpread = movementVariation + maxFiringSpread;
		
		//Finds firing spread if aiming
        if (Input.GetButton("Fire2"))
        {
            firingSpread += (firingSpreadRate/2 * Time.deltaTime);
        }
        else
        {
            firingSpread += (firingSpreadRate * Time.deltaTime);
        }

        //Prevents firingSpread from going over capped value
        if (Input.GetButton("Fire2"))
        {
            firingSpread = Mathf.Clamp(firingSpread, -modifiedMaxFiringSpread / 2, modifiedMaxFiringSpread / 2);
        }
        else
        {
            firingSpread = Mathf.Clamp(firingSpread, -modifiedMaxFiringSpread, modifiedMaxFiringSpread);
        }
		
        return firingSpread;
    }

    //Receives input
    private void MyInput()
    {
        //Checks input based on full auto or not
        if (fullAuto)
        {
            shooting = Input.GetKey(KeyCode.Mouse0);
        }
        else
        {
            shooting = Input.GetKeyDown(KeyCode.Mouse0);
        }


        //Changes the variability in spread when continously firing weapon
        if ((readyToShoot | bulletsLeft <= 0) && firingSpread > 0)
        {
            firingSpread -= (firingSpreadRate * Time.deltaTime);
        }      

        else
        {
            findShootingVariation();
        }

        
        

        //Changes firemode
        if (Input.GetKeyDown(KeyCode.V) && fullAutoAllowed)
        {
            if (fullAuto) { fullAuto = false; }
            else { fullAuto = true; }
        }

        //Reload or fix malfunctions
        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading && !isMalfunction)
        {
            Reload();
        }
		
		//Checks if need to fix malfunction
        else if (Input.GetKeyDown(KeyCode.R) && isMalfunction)
        {
            fixMalfunction();
        }
		
		//If conditions are satisifed. Fire rate, not shooting already, not reloading and has ammo
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = bulletsPerTap;
            Shoot();
        }
    }
	
	//Scales damage based on distance away from target
	private float findDistanceMultiplier(float distance)
	{	
		
		//Checks if distance is longer than effective range <- No damage dropoff
		if ((distance*2) > effectiveRange)
		{
			//Finds multiplier based on how far from max
			float distancePenalty = (maxRange-distance)/maxRange;
			return distancePenalty;
		}
		else
		{
			//No Distance Penalty (Distance is within Effective Range)
			return 1;
		}
	}
	
    //Shoots Raycasts for collision
    private void Shoot()
    {
        readyToShoot = false;
		
		//Camera Recoil
		camHolder.GetComponentInParent<CamRecoil>().Fire(recoilCam, recoilAimCam, recoilCamSpeed, recoilCamRecoverySpeed);
		GetComponentInParent<GunKick>().Fire();
		
        //Spread/Accuracy
		float spreadX = Random.Range(-horizontalSpread - firingSpread, horizontalSpread + firingSpread);
		float spreadY = Random.Range(-verticalSpread - firingSpread, verticalSpread + firingSpread);

		// Adjust direction based on camera's orientation
		Vector3 spreadOffset = (fpsCam.transform.right * spreadX) + (fpsCam.transform.up * spreadY);
		Vector3 direction = fpsCam.transform.forward + spreadOffset;
		direction.Normalize(); // Normalize to maintain proper direction

        //Clones Shell Casings
        if (!isMalfunction) { Instantiate(shellPrefab, shellEjectionPoint.position, shellEjectionPoint.rotation); }
        checkMalfunction(probabilityOfMalfunction, 0f);

        //Checks if raycast hit anything
        if (Physics.Raycast(fpsCam.transform.position, direction, out rayHit, maxRange, enemyDef) && (!isMalfunction))
        {
			//Starts trail function
            StartCoroutine(PlayTrail(TrailLeave.transform.position, rayHit.point, rayHit));
			
			//Finds distance penalty
			float distance = Vector3.Distance(rayHit.collider.transform.position, attackPoint.transform.position);
			float distanceMultiplier = findDistanceMultiplier(distance);
			
			//Checks if raycast hit an enemy
            if (rayHit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy")) // Check if it's on the Enemy layer
            {
                if (rayHit.collider.CompareTag("Head")) // Check if it's a headshot
                {
                    rayHit.collider.GetComponentInParent<GuardAI>().takeDamage(Mathf.RoundToInt(headDamage*distanceMultiplier), armorPenetration, bulletPower);
                }
                else if (rayHit.collider.CompareTag("Body")) // Check if it's a body shot
                {
                    rayHit.collider.GetComponentInParent<GuardAI>().takeDamage(Mathf.RoundToInt(bodyDamage*distanceMultiplier), armorPenetration, bulletPower);
                }
                else if (rayHit.collider.CompareTag("Legs")) // Check if it's a leg shot
                {
                    rayHit.collider.GetComponentInParent<GuardAI>().takeDamage(Mathf.RoundToInt(legDamage*distanceMultiplier), armorPenetration, bulletPower);
                }
                else if (rayHit.collider.CompareTag("Arms")) // Check if it's an arm shot
                {
                    rayHit.collider.GetComponentInParent<GuardAI>().takeDamage(Mathf.RoundToInt(armDamage*distanceMultiplier), armorPenetration, bulletPower);
                }
            }

        }

        else 
        { 
            if (!isMalfunction)
            {
                StartCoroutine(PlayTrail(TrailLeave.transform.position, TrailLeave.transform.position + (direction * MissDistance), new RaycastHit()));
            }
        }
		
        bulletsLeft--;
        bulletsShot--;

        //Executes function with delay
        Invoke("ResetShooting", timeBetweenShots);
		
		
        if (bulletsShot > 0  && (bulletsLeft > 0) && magazinesLeft > 0 && !isMalfunction)
        {
            //Executes the shoot function and has cooldown of firerate (TBS)
            
            Invoke("Shoot", timeBetweenShots);
        }
		
        else if (isMalfunction)
        {
            Debug.Log("Gun Malfunction");
        }
    }

    


    //MALFUNCTIONS
    private void fixMalfunction()
    {
        isMalfunction = false;
    }
	
	//Checks if the gun should malfunction based on chance
    private void checkMalfunction(float malChance, float damage)
    {
        float chanceOfMalfunction = 1 - (malChance);
        float random = Random.Range(0f,1f);
        if (random > chanceOfMalfunction)
        {
            Debug.Log("Misfire Malfunction");
        }
        
        chanceOfMalfunction = 1 - (malChance + damage);
        random = Random.Range(0f,1f);
        if (random > chanceOfMalfunction)
        {
            isMalfunction = true;
        }
        else
        {
            //Debug.Log("Malfunciton Check Failed");
        }
    }

	
    //Creates the trails
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

	//Dislays the trails
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

    //Camera Shake for explosions
    private IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPos = fpsCam.transform.localPosition;
        float elapsed = 0.0f;
		
		//Shakes camera for set duration
        while (elapsed < duration)
        {
			//Rapidly changes position of camera
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
			
            fpsCam.transform.localPosition = originalPos + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }
		
		//Resets camera back to original position
        fpsCam.transform.localPosition = originalPos;
    }
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // Start is called before the first frame update
    
    void Start()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;

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

    // Update is called once per frame
    void Update()
    {
        MyInput();
    }
	
	
}
