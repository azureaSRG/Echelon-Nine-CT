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

    //Casing Ejection
    public GameObject shellPrefab;
    public Transform shellEjectionPoint;

    //Bullet Drop Variables
    public float bulletLifetime;
    public float gravityForce;

    //Ammo Information
    public string caliber;
    public int magazineSize;
    public int magazineReserves;
    public bool fullAutoAllowed;
    public bool fullAuto;

    //Magazine Information
    private int[] magList = new int[] {0};
    private bool isChambered = true;

    //Damage Information
    public int headDamage, bodyDamage, legDamage, armDamage;
    public float armorPenetration;

    //Firing Information
    public float timeBetweenShots, muzzleVelocity, effectiveRange, maxRange;

    //Gun Information
    public float cost, mass, probabilityOfMalfunction;

    //Recoil Information
    public float horizontalRecoil;
    public float verticalRecoil;

    //Handling Information
    public float fullReloadTime,partialReloadTime, adsTime, equipSpeed;

    //Accuracy Information
    public float horizontalSpread;
    public float verticalSpread;

    /*
    Weapon Specific Information:
    These variables are changed based on the usage
    */

    private float firingSpread;

    private int magazinesLeft, bulletsLeft, bulletsShot, bulletsPerTap;
    private bool shooting, readyToShoot, reloading;

    //Malfunctions
    private bool isMalfunction;
    private int malfunctionType;

    //References
    public Camera fpsCam;
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
    //public ImpactType ImpactType;

    public float MissDistance = 100f;
    public float SimulationSpeed = 200f;
    private ObjectPool<TrailRenderer> TrailPool;

    private void ResetShooting()
    {
        readyToShoot = true;
    }
    
    private void RefillAmmo()
    {
        magazinesLeft = magazineReserves;
        for (int x = 0; x >= magazineReserves; x++)
        {
            magList = new int[] { magazineSize };
        }
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
        
        if ((!shooting | bulletsLeft <= 0) && firingSpread > 0)
        {
            firingSpread -= (0.03f * Time.deltaTime);
        }

        else
        {
            firingSpread += (0.03f * Time.deltaTime);
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

        else if (Input.GetKeyDown(KeyCode.R) && isMalfunction)
        {
            fixMalfunction();
        }

        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = bulletsPerTap;
            Shoot();
        }
    }
    
    //Sets reloading and delays reload finsihed function by time
    private void Reload()
    {
        reloading = true;
        if (bulletsLeft > 0) {
            Invoke("ReloadFinished", partialReloadTime); }
        else {
            Invoke("ReloadFinished", fullReloadTime); 
        }

    }

    //Shoots raycasts
    private void Shoot()
    {
        readyToShoot = false;

        //Spread/Accuracy
        float spreadX = Random.Range(-horizontalSpread-firingSpread, horizontalSpread+firingSpread);
        float spreadY = Random.Range(-verticalSpread-firingSpread, verticalSpread+firingSpread);

        Vector3 direction = fpsCam.transform.forward + new Vector3(spreadX, spreadY, 0);

        //Clones Shell Casings
        if (!isMalfunction) { Instantiate(shellPrefab, shellEjectionPoint.position, shellEjectionPoint.rotation); }
        checkMalfunction(probabilityOfMalfunction, 0f);

        //Raycast
        if (Physics.Raycast(fpsCam.transform.position, direction, out rayHit, effectiveRange, enemyDef) && (!isMalfunction))
        {
            Debug.Log(rayHit.collider.name);
            Debug.Log("Raycast Hit");

            StartCoroutine(PlayTrail(TrailLeave.transform.position, rayHit.point, rayHit));
           /*
            if (rayHit.collider.CompareTag("Enemy"))
            {
                rayHit.collider.GetComponent<Enemy>().TakeDamage(headDamage);
            }
            */
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

        if (bulletsShot > 0  && (bulletsLeft > 0 | isChambered) && magazinesLeft > 0 && !isMalfunction)
        {
            //Executes the shoot function and has cooldown of firerate (TBS)
            
            Invoke("Shoot", timeBetweenShots);
            if (bulletsLeft <= 0)
            {
                isChambered = false;
            }
        }

        else if (isMalfunction)
        {
            Debug.Log("Gun Malfunction");
        }
    }

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

    //Sets bullets left to the mag size then sets reloading to false
    private void ReloadFinished()
    {
        if (bulletsLeft == 0)
        {
            magazinesLeft--;   
        }
        if (!isChambered)
        {
            isChambered = true;
            bulletsLeft--;
        }
        
        bulletsLeft = magazineSize;
        reloading = false;
    }
    
    private void fixMalfunction()
    {
        isMalfunction = false;
    }

    private void checkMalfunction(float malChance, float damage)
    {
        float chanceOfMalfunction = 1 - (malChance);
        float random = Random.Range(0f,1f);
        Debug.Log(random);
        if (random > chanceOfMalfunction)
        {
            Debug.Log("Misfire Malfunction");
        }
        
        chanceOfMalfunction = 1 - (malChance + damage);
        random = Random.Range(0f,1f);
        Debug.Log(random);
        if (random > chanceOfMalfunction)
        {
            /*
            1 Failure to Feed
            2 Failure to Eject
            3 Failure to Extract
            4 Out of Battery
            5 Misfire
            */
            Debug.Log("Weapon Malfunction");
            isMalfunction = true;
            /*
            if (damage > value)
            {
                Debug.Log("Failure to");
            }
            
            else if (damage > value)
            {
                Debug.Log("Failure to");
            }

            ...
            */
        }
        else
        {
            Debug.Log("Malfunciton Check Failed");
        }
    }


    //Trail Renderers
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
    // Update is called once per frame
    void Update()
    {
        MyInput();
    }
}
