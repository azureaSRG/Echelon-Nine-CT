using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public bool fullAuto;

    //Magazine Information
    private int[] magList = new int[] {0};

    //Damage Information
    public int headDamage, bodyDamage, legDamage, armDamage;
    public float armorPenetration;

    //Firing Information
    public float timeBetweenShots, muzzleVelocity, effectiveRange, maxRange;

    //Gun Information
    public float cost, mass, probabiltyOfMalfunction;

    //Recoil Information
    public float horizontalRecoil;
    public float verticalRecoil;

    //Handling Information
    public float reloadTime, adsTime, equipSpeed;

    //Accuracy Information
    public float horizontalSpread;
    public float verticalSpread;
    

    /*
    Weapon Specific Information:
    These variables are changed based on the usage
    */

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
        if (fullAuto)
        {
            shooting = Input.GetKey(KeyCode.Mouse0);
        }
        else
        {
            shooting = Input.GetKeyDown(KeyCode.Mouse0);
        }

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading)
        {
            Reload();
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
        Invoke("ReloadFinished", reloadTime);

    }

    //Shoots raycasts
    private void Shoot()
    {
        readyToShoot = false;

        //Spread/Accuracy
        float spreadX = Random.Range(-horizontalSpread, horizontalSpread);
        float spreadY = Random.Range(-verticalSpread, verticalSpread);

        Vector3 direction = fpsCam.transform.forward + new Vector3(spreadX, spreadY, 0);

        //Clones Shell Casings
        Instantiate(shellPrefab, shellEjectionPoint.position, shellEjectionPoint.rotation);
        checkMalfunction(probabilityOfMalfunction, 1)
        
        //Raycast
        if (Physics.Raycast(fpsCam.transform.position, direction, out rayHit, effectiveRange, enemyDef))
        {
            Debug.Log(rayHit.collider.name);
            Debug.Log("Raycast Hit");
            /*
            if (rayHit.collider.CompareTag("Enemy"))
            {
                rayHit.collider.GetComponent<ShootingAi>().TakeDamage(headDamage);
            }
            */
        }

        bulletsLeft--;
        bulletsShot--;

        //Executes function with delay
        Invoke("ResetShooting", timeBetweenShots);

        if (bulletsShot > 0 && bulletsLeft > 0 && magazinesLeft > 0 && !isMalfunction)
        {
            //Executes the shoot function and has cooldown of firerate (TBS)
            Invoke("Shoot", timeBetweenShots);
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
    }

    //Sets bullets left to the mag size then sets reloading to false
    private void ReloadFinished()
    {
        if (bulletsLeft = 0)
        {
            magazinesLeft--;   
        }
        
        bulletsLeft = magazineSize;
        reloading = false;
    }
    
    private void fixMalfunction()
    {
        isMalfunction = false;
    }

    private int checkMalfunction(float malChance, float damage)
    {
        chanceOfMalfunction = 1 - (malChance);
        random = Random.Range(0,1);
        if (random >= chanceOfMalfunction)
        {
            Debug.Log("Misfire Malfunction")l
        }
        
        chanceOfMalfunction = 1 - (malChance + damage);
        random = Random.Range(0,1);
        if (random >= chanceOfMalfunction)
        {
            /*
            1 Failure to Feed
            2 Failure to Eject
            3 Failure to Extract
            4 Out of Battery
            5 Misfire
            */
            Debug.Log("Weapon Malfunction");
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
    
    // Update is called once per frame
    void Update()
    {
        MyInput();
    }
}
