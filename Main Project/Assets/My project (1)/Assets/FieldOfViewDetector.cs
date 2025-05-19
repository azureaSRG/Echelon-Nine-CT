using System.Collections.Generic;
using UnityEngine;

public class FieldOfViewDetector : MonoBehaviour
{
    [Header("View Settings")]
    public float viewRadius = 15f;
    [Range(0, 360)] public float viewAngle = 90f;
    public LayerMask targetMask;
    public LayerMask obstacleMask;

    private EnemyTracker tracker;
    private HashSet<Transform> currentlyVisibleEnemies = new HashSet<Transform>();

    private void Awake()
    {
        tracker = GetComponent<EnemyTracker>();
    }

    private void Update()
    {
        DetectEnemies();
    }

    private void DetectEnemies()
    {
        if (tracker == null) return;

        Collider[] targetsInView = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
        HashSet<Transform> newlyVisible = new HashSet<Transform>();

        foreach (Collider target in targetsInView)
        {
            Transform enemy = target.transform;
            Vector3 dirToEnemy = (enemy.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, dirToEnemy) < viewAngle / 2)
            {
                float distanceToEnemy = Vector3.Distance(transform.position, enemy.position);

                if (!Physics.Raycast(transform.position, dirToEnemy, distanceToEnemy, obstacleMask))
                {
                    newlyVisible.Add(enemy);

                    if (!currentlyVisibleEnemies.Contains(enemy))
                    {
                        tracker.RegisterEnemy(enemy);
                    }
                }
            }
        }

        foreach (Transform previouslySeen in currentlyVisibleEnemies)
        {
            if (!newlyVisible.Contains(previouslySeen))
            {
                tracker.UnregisterEnemy(previouslySeen);
            }
        }

        currentlyVisibleEnemies = newlyVisible;
    }
}
