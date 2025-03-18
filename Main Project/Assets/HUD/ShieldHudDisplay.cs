using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShieldHUD : MonoBehaviour
{
  public GameObject shieldPrefab;
  public Transform shieldContainer;
  //Set these values to be equal to the shields stat in playerStats
  public int currentArmor = 0;
  public int maxArmorSlots = 0;

  private List<GameObject> Shields = new List<GameObject>();

  void Start()
  {
    DrawShieldHud(currentArmor, maxArmorSlots);
  }

  void Update()
  {
    DrawShieldHud(currentArmor, maxArmorSlots);
  }

  void DrawShieldHud(int currentArmorPlates, int maxArmorSlots)
  {
    float width = 380f / maxArmorSlots;
    float x_offset = 10f;

    if (Shields.Count < maxArmorSlots)
    {
      for (int i = Shields.Count; i < maxArmorSlots; i++)
      {
        GameObject armorPlate = Instantiate(shieldPrefab, shieldContainer);
        Shields.Add(armorPlate);
      }
    }
    else if (Shields.Count > maxArmorSlots)
    {
      for (int i = Shields.Count - 1; i >= maxArmorSlots; i--)
      {
        Destroy(Shields[i]);
        Shields.RemoveAt(i);
      }
    }

    for (int i = 0; i < maxArmorSlots; i++)
    {
      Image shieldImage = Shields[i].GetComponent<Image>();
      shieldImage.color = (i < currentArmorPlates) ? Color.cyan : Color.gray;

      RectTransform rt = Shields[i].GetComponent<RectTransform>();
      rt.sizeDelta = new Vector2(width, 20f);
      rt.anchoredPosition = new Vector2(x_offset + i * width, 0);
    }
  }
}
