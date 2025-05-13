using UnityEngine;

public class DebugDireection : MonoBehaviour
{
    public Transform muzzle;
    public Transform target;
    public float range = 100f;
    public float hSpread = 5f;
    public float vSpread = 5f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Shoot();
    }

    void Shoot()
    {
        Vector3 aimPoint = target.position + Vector3.up * 1.5f;
        Vector3 baseDir = (aimPoint - muzzle.position).normalized;

        Quaternion spread = Quaternion.Euler(
            Random.Range(-vSpread, vSpread),
            Random.Range(-hSpread, hSpread),
            0f
        );
        Vector3 finalDir = spread * baseDir;

        Debug.Log("Direction: " + finalDir);
        Debug.DrawRay(muzzle.position, finalDir * range, Color.red, 3f);

        if (Physics.Raycast(muzzle.position, finalDir, out RaycastHit hit, range))
        {
            Debug.Log("Hit " + hit.collider.name);
        }
    }
}
