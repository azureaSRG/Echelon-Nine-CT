using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyIconPool : MonoBehaviour
{
    private Image enemyPrefab;
    private Transform iconsParent;
    private Queue<Image> pool = new Queue<Image>();

    public void Initialize(Image prefabToUse, Transform parentToUse)
    {
        enemyPrefab = prefabToUse;
        iconsParent = parentToUse;
    }

    public Image GetIcon()
    {
        Image icon;
        if (pool.Count > 0)
        {
            icon = pool.Dequeue();
        }
        else
        {
            icon = Instantiate(enemyPrefab, iconsParent);
        }

        icon.gameObject.SetActive(true);
        icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, 1f); // Reset to fully visible
        return icon;
    }

    public void ReturnIcon(Image icon)
    {
        icon.gameObject.SetActive(false);
        pool.Enqueue(icon);
    }
}
