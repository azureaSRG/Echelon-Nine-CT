using UnityEngine;
using UnityEngine.UI;

public class HUDBar : MonoBehaviour
{
  [SerializeField] private PlayerStats playerStats;
  public Image healthBarImage;
  public float maxValue;
  public float currentValue;
  public Color fullColor = Color.green;
  public Color emptyColor = Color.red;

  void Start()
  {
    maxValue = playerStats.maxHealth
    currentValue = playerStats.health;
  }

  void Update()
  {
    currentValue = playerStats.health;

    currentValue = Mathf.Clamp(currentValue, 0, maxValue);

    barImage.fillAmount = currentValue/maxValue;

    healthBarImage.color = Color.Lerp(emptyColor, fullColor, currentValue/maxValue);
  }
}
