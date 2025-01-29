using UnityEngine;

[CreateAssetMenu(fileName = "GunConfig", menuName = "Guns/GunConfig", order = 2)]
public class GunConfig : ScriptableObject
{
    //Ammo Information
    public string caliber;
    public int magazineSize;
    public int magazineReserves;
    public bool fullAuto;

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
    public float reloadTime, adsTime, equipSpeed;

    //Accuracy Information
    public float horizontalSpread;
    public float verticalSpread;
}
