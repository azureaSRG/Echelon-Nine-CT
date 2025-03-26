using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CamRecoil : MonoBehaviour
{
	public float rotationSpeed, returnSpeed;
	public Vector3 recoilRotation = new Vector3(2f,2f,2f);
	public Vector3 recoilRotationAiming = new Vector3(0.5f,0.5f,0.5f);
	public bool aiming;
	
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
	
	public void Fire()
	{
		if (aiming)
		{
			currentRotation += new Vector3(-recoilRotationAiming.x, Random.Range(-recoilRotationAiming.y, recoilRotationAiming.y), Random.Range(-recoilRotationAiming.z,recoilRotationAiming.z));
		}
		else
		{
			currentRotation += new Vector3(-recoilRotation.x, Random.Range(-recoilRotation.y, recoilRotation.y) ,Random.Range(-recoilRotation.z, recoilRotation.z));
		}
		
	}

    // Update is called once per frame
    void Update()
    {
		if (Input.GetButton("Fire2")) {aiming = true;}
		else {aiming = false;}
    }
}
