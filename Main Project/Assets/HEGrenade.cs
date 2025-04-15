using UnityEngine;

public class HEGrenade : MonoBehaviour
{
	
	private float countdown;
	private float delay = 5f;
	private float explosionRadius = 30f;
	private float armorPenetration = 0.50f;
	private int power = 7;
	private float damage = 5000f;
	public GameObject explosionEffect;
	bool hasExploded = false;
	
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        countdown = delay;
    }

    // Update is called once per frame
    void Update()
    {
        countdown -= Time.deltaTime;
		if (countdown <= 0f && !hasExploded)
		{
			Explode();
			hasExploded = true;
		}
    }
	
	void Explode()
	{
		Debug.Log("Effect");
		Instantiate(explosionEffect, transform.position, transform.rotation);
		hasExploded = true;
		
		Collider[] objectsInRadius = Physics.OverlapSphere(transform.position, explosionRadius);
		foreach (Collider c in objectsInRadius)
		{
			RaycastHit hit;
			if (Physics.Raycast(transform.position, (c.transform.position - transform.position).normalized, out hit, explosionRadius))
			{
				float distance = Vector3.Distance(c.transform.position, transform.position);
				float scaledDamage = damage * (1 - distance/explosionRadius);
				if (c.gameObject.layer == LayerMask.NameToLayer("Enemy"))
				{
					c.GetComponentInParent<GuardAI>().takeDamage(Mathf.RoundToInt(scaledDamage), armorPenetration, power);
				}
			}
			else{
				Debug.Log("Behind Wall");
			}
		}
		
		Destroy(gameObject);
	}
}
