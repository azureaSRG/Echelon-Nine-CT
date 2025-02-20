using UnityEngine;

public class GunAim : MonoBehaviour
{
    public Transform activeWeapon;

    public Transform defaultPosition;
    public Transform adsPosition;
    public Vector3 weaponPosition; // set to 0 0 0 in inspector

    public float aimSpeed = 0.25f; // time to enter ADS
    public float prevDefaultFOV = 60f; // FOV in degrees
    public float zoomRatio = 0.5f; // 1/zoom times

    public Camera fpsCam; // player camera
    public Camera clipCam; // gun camera

    void Update()
    {
        // ADS camera and gun movement
        if (Input.GetButton("Fire2"))
        {
            weaponPosition = Vector3.Lerp(weaponPosition, adsPosition.localPosition, aimSpeed * Time.deltaTime);
            activeWeapon.localPosition = weaponPosition;
            SetFieldOfView(Mathf.Lerp(fpsCam.fieldOfView, zoomRatio * prevDefaultFOV, aimSpeed * Time.deltaTime));
            SetFieldOfView(Mathf.Lerp(clipCam.fieldOfView, zoomRatio * prevDefaultFOV, aimSpeed * Time.deltaTime));
        }
        else
        {
            weaponPosition = Vector3.Lerp(weaponPosition, defaultPosition.localPosition, aimSpeed * Time.deltaTime);
            activeWeapon.localPosition = weaponPosition;
            SetFieldOfView(Mathf.Lerp(fpsCam.fieldOfView, prevDefaultFOV, aimSpeed * Time.deltaTime));
            SetFieldOfView(Mathf.Lerp(clipCam.fieldOfView, prevDefaultFOV, aimSpeed * Time.deltaTime));
        }
    }

    void SetFieldOfView(float fov)
    {
        fpsCam.fieldOfView = fov;
        clipCam.fieldOfView = fov;
    }
}