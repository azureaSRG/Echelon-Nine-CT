using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CamRecoil : MonoBehaviour
{
	private float rotationSpeed, returnSpeed;
	private Vector3 recoilRotation = new Vector3(2f,2f,2f);
	private Vector3 recoilRotationAiming = new Vector3(0.5f,0.5f,0.5f);
	private bool aiming;
	
	private Vector3 currentRotation;
	private Vector3 Rot;
	
	void Start()
	{
	}
	
	void FixedUpdate()
	{
		currentRotation = Vector3.Lerp(currentRotation, Vector3.zero, returnSpeed * Time.deltaTime);
		Rot = Vector3.Slerp(Rot, currentRotation, rotationSpeed * Time.fixedDeltaTime);
		transform.localRotation = Quaternion.Euler(Rot);
	}
	
	public void Fire(Vector3 recoilRot, Vector3 recoilRotAiming, float speed, float recoverySpeed)
	{
		returnSpeed = recoverySpeed;
		rotationSpeed = speed;
		if (aiming)
		{
			currentRotation += new Vector3(-recoilRotAiming.x, Random.Range(-recoilRotAiming.y, recoilRotAiming.y), Random.Range(-recoilRotAiming.z,recoilRotAiming.z));
		}
		else
		{
			currentRotation += new Vector3(-recoilRot.x, Random.Range(-recoilRot.y, recoilRot.y) ,Random.Range(-recoilRot.z, recoilRot.z));
		}
		
	}

    // Update is called once per frame
    void Update()
    {
		if (Input.GetButton("Fire2")) {aiming = true;}
		else {aiming = false;}
    }
}
