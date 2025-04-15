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

  //This is a list that gets created for all of the players shield plates
  private List<GameObject> Shields = new List<GameObject>();

  void Start()
  {
    DrawShieldHud(currentArmor, maxArmorSlots);
  }

  //Gets called every second to updatet the display, 
  //this is because as of the creation of this code, no player damage existed yet
  void Update()
  {
    DrawShieldHud(currentArmor, maxArmorSlots);
  }

  //Takes the shield data that gets assigned from player stats and draws the amount of 
  //shield plates that corresponds to that value.
  void DrawShieldHud(int currentArmorPlates, int maxArmorSlots)
  {
    //divides the total length of the armor plate bar by the max armor plates
    float width = 380f / maxArmorSlots;
    float x_offset = 10f;

    // if the current amount of armor plates are less than the max amount then
    // more shields get added to the total list of armor plates
    if (Shields.Count < maxArmorSlots)
    {
      for (int i = Shields.Count; i < maxArmorSlots; i++)
      {
        GameObject armorPlate = Instantiate(shieldPrefab, shieldContainer);
        Shields.Add(armorPlate);
      }
    }
    // if the list of shields are greater than the max amount of armor slots than
    // it removes shields from the list and destroys the extra object from the screen
    else if (Shields.Count > maxArmorSlots)
    {
      for (int i = Shields.Count - 1; i >= maxArmorSlots; i--)
      {
        Destroy(Shields[i]);
        Shields.RemoveAt(i);
      }
    }

    //Changes color and assigns sized to each visible armor plate accordingly
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
