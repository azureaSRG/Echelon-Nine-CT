using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerStats : MonoBehaviour
{
    [SerializeField]
    protected int health;
    [SerializeField]
    protected int maxHealth;
    
    [SerializeField]
    protected int maxArmorPoints;
    public float armorResistance;
    private int armorPoints;
    public bool isPlated;

    public bool alive;
    public int experience;
    public int playerLevel;
    [SerializeField]
    public int neededXP = 0;
    private int[] xpIntervals = new int[] {1000,2000,3000,4000,5000,10000,15000,20000,30000,40000,49999};
    private int[] levelIntervals = new int[] { 10, 20, 30, 40, 50, 60, 70, 80, 90, 96, 98 };
    //10,20,30,40,50,60,70,80,90,96,98

    public void gainExperience(int xp)
    {
        int gainedXP = xp += experience;
        experience = gainedXP;
        checkLevelUp();
    }
    
    public void checkLevelUp()
    {
        foreach (int element in levelIntervals)
        {
            int i = 0;
            if (element > playerLevel)
            {
                int neededXP = levelIntervals[i];
            }
            else
            {
                i++;   
            }
        }

        if (experience >= neededXP)
        {
            playerLevel++;
            experience -= neededXP;
            checkLevelUp();
        }
    }
    
    public void checkHealth()
    {
        if (health < 0)
        {
            isDead();
            health = 0;
        }

        if (health > maxHealth)
        {
            health = maxHealth;
        }

    }

    public void isDead()
    {
        alive = false;
    }

    public void restoreArmor(int restoration)
    {
        if (isPlated && armorPoints < maxArmorPoints)
        {
            armorPoints += restoration;
        }
        else
        {
            Debug.LogWarning("Armor Not Plated! or Armor Full!");
        }
    }
    
    public void damagePlayer(int damage, float armorPeircingValue)
    {
        if (armorResistance >= armorPeircingValue && armorPoints > 0)
            {
            int damagedHealth = health - Mathf.RoundToInt(damage/armorResistance);
            health = damagedHealth;
            armorPoints--;
            }
        else
        {
            int damagedHealth = health - damage;
        }
        //Debug.Log(health);
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = maxHealth;
        armorPoints = maxArmorPoints;
    }
}
