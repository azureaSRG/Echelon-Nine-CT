using UnityEngine;

public class YeetGrenade : MonoBehaviour
{
	public float throwForce;
	public GameObject grenadePrefab;
	
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
		{
			ThrowGrenade();
		}
    }
	
	private void ThrowGrenade()
	{
		GameObject grenade = Instantiate(grenadePrefab, transform.position, transform.rotation);
		Rigidbody rb = grenade.GetComponent<Rigidbody>();
		rb.AddForce(transform.forward * throwForce);
		rb.AddTorque(transform.right * throwForce);
	}
}
