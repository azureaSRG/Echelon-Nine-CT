using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyTracker : MonoBehaviour
{
    [Header("Minimap Settings")]
    public RectTransform minimapRectTransform;
    public RectTransform enemyIconsParent;
    public GameObject enemyIconPrefab;
    public float fadeDuration = 2f;
    public float iconLifespan = 5f;

    private Dictionary<Transform, EnemyIconData> enemyToIconMap = new Dictionary<Transform, EnemyIconData>();
    private Queue<GameObject> iconPool = new Queue<GameObject>();

    private class EnemyIconData
    {
        public GameObject icon;
        public float timeSinceSeen;
        public bool isVisible;
    }

    private void Update()
    {
        List<Transform> enemies = new List<Transform>(enemyToIconMap.Keys);
        foreach (Transform enemy in enemies)
        {
            if (enemy == null)
                continue;

            EnemyIconData data = enemyToIconMap[enemy];

            // Always follow enemy position
            data.icon.transform.localPosition = WorldToMinimapPosition(enemy.position);

            if (data.isVisible)
            {
                data.timeSinceSeen = 0f;
                SetIconAlpha(data.icon, 1f);
            }
            else
            {
                data.timeSinceSeen += Time.deltaTime;

                float alpha = Mathf.Clamp01(1 - (data.timeSinceSeen / fadeDuration));
                SetIconAlpha(data.icon, alpha);

                if (data.timeSinceSeen >= iconLifespan)
                {
                    data.icon.SetActive(false);
                    iconPool.Enqueue(data.icon);
                    enemyToIconMap.Remove(enemy);
                }
            }
        }
    }

    public void RegisterEnemy(Transform enemy)
    {
        if (!enemyToIconMap.TryGetValue(enemy, out var data))
        {
            GameObject icon = iconPool.Count > 0 ? iconPool.Dequeue() : Instantiate(enemyIconPrefab, enemyIconsParent);
            icon.transform.SetParent(enemyIconsParent);
            icon.transform.localPosition = WorldToMinimapPosition(enemy.position);
            icon.SetActive(true);
            SetIconAlpha(icon, 1f);

            enemyToIconMap[enemy] = new EnemyIconData
            {
                icon = icon,
                timeSinceSeen = 0f,
                isVisible = true
            };
        }
        else
        {
            data.isVisible = true;
        }
    }

    public void UnregisterEnemy(Transform enemy)
    {
        if (enemyToIconMap.TryGetValue(enemy, out var data))
        {
            data.isVisible = false;
        }
    }

    private Vector3 WorldToMinimapPosition(Vector3 worldPos)
    {
        if (minimapRectTransform == null)
        {
            return Vector3.zero;
        }

        Vector3 offset = worldPos - transform.position;
        float mapWidth = minimapRectTransform.rect.width;
        float mapHeight = minimapRectTransform.rect.height;

        Vector2 minimapPos = new Vector2(offset.x / 20f, offset.z / 20f);
        minimapPos *= Mathf.Min(mapWidth, mapHeight) * 0.5f;

        return new Vector3(minimapPos.x, minimapPos.y, 0);
    }

    private void SetIconAlpha(GameObject icon, float alpha)
    {
        if (icon.TryGetComponent<Image>(out Image img))
        {
            Color color = img.color;
            color.a = alpha;
            img.color = color;
        }
    }
}
