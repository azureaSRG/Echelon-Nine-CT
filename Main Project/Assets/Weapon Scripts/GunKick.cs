using UnityEngine;

public class GunKick : MonoBehaviour
{
    public Transform recoilPosition;
	public Transform rotationPoint;
	
	public float positionalRecoilSpeed = 8f;
	public float rotationRecoilSpeed = 8f;
	
	public float positionalReturnSpeed = 18f;
	public float rotationalReturnSpeed = 38f;
	
	public Vector3 RecoilRotation = new Vector3(10,5,7);
	public Vector3 RecoilKickback = new Vector3(0.015f,0f, -0.2f);
	
	public Vector3 RecoilRotationAim = new Vector3(10,4,6);
	public Vector3 RecoilKickbackAim = new Vector3(0.015f,0f,-0.2f);
	
	Vector3 rotationalRecoil, positionalRecoil, Rot;
	public bool aiming;
	
	public void Fire()
	{
		if (aiming)
		{
			rotationalRecoil += new Vector3(-RecoilRotationAim.x, Random.Range(-RecoilRotationAim.y,RecoilRotationAim.y), Random.Range(-RecoilRotationAim.z, RecoilRotationAim.z));
			positionalRecoil += new Vector3(-RecoilKickbackAim.x, Random.Range(-RecoilKickbackAim.y,RecoilKickbackAim.y), Random.Range(-RecoilKickbackAim.z, 0f));
			
		}
		else
		{
			rotationalRecoil += new Vector3(-RecoilRotation.x, Random.Range(-RecoilRotation.y,RecoilRotation.y), Random.Range(-RecoilRotation.z, RecoilRotation.z));
			positionalRecoil += new Vector3(-RecoilKickback.x, Random.Range(-RecoilKickback.y,RecoilKickback.y), Random.Range(-RecoilKickback.z, 0f));
		}
	}
	
	void FixedUpdate()
	{
		rotationalRecoil = Vector3.Lerp(rotationalRecoil, Vector3.zero, rotationalReturnSpeed*Time.deltaTime);
		positionalRecoil = Vector3.Lerp(positionalRecoil, Vector3.zero, positionalRecoilSpeed*Time.deltaTime);
		
		recoilPosition.localPosition = Vector3.Slerp(recoilPosition.localPosition, positionalRecoil, positionalRecoilSpeed*Time.deltaTime);
		Rot = Vector3.Slerp(Rot, rotationalRecoil, rotationRecoilSpeed*Time.fixedDeltaTime);
		rotationPoint.localRotation = Quaternion.Euler(Rot);
	}
	
	void Update()
	{
		if (Input.GetButton("Fire2"))
		{
			aiming = true;
		}
		else
		{
			aiming = false;
		}
	}
}