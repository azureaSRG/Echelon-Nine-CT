using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShieldHUD : MonoBehaviour
{
    public GameObject shieldPrefab;
    public Transform shieldContainer;
    public PlayerStats playerStats;

    private List<GameObject> Shields = new List<GameObject>();

    void Start()
    {
        // Make sure playerStats is assigned
        if (playerStats == null)
        {
            playerStats = FindObjectOfType<PlayerStats>();
        }

        DrawShieldHud(playerStats.armorPoints, playerStats.maxArmorPoints);
    }

    void Update()
    {
        DrawShieldHud(playerStats.armorPoints, playerStats.maxArmorPoints);
    }

    void DrawShieldHud(int currentArmorPlates, int maxArmorPoints)
    {
        float width = 380f / maxArmorPoints;
        float x_offset = 10f;

        if (Shields.Count < maxArmorPoints)
        {
            for (int i = Shields.Count; i < maxArmorPoints; i++)
            {
                GameObject armorPlate = Instantiate(shieldPrefab, shieldContainer);
                Shields.Add(armorPlate);
            }
        }
        else if (Shields.Count > maxArmorPoints)
        {
            for (int i = Shields.Count - 1; i >= maxArmorPoints; i--)
            {
                Destroy(Shields[i]);
                Shields.RemoveAt(i);
            }
        }

        for (int i = 0; i < maxArmorPoints; i++)
        {
            Image shieldImage = Shields[i].GetComponent<Image>();
            shieldImage.color = (i < currentArmorPlates) ? Color.cyan : Color.gray;

            RectTransform rt = Shields[i].GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(width, 20f);
            rt.anchoredPosition = new Vector2(x_offset + i * width, 0);
        }
    }
}