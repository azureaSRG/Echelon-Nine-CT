using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerStats : MonoBehaviour
{
    [SerializeField]
    public int health;
    [SerializeField]
    public int maxHealth;
    
    public int maxArmorPoints;
	public int armorPoints;
    public float armorResistance;
    public float stoppingPower;
    public bool isPlated;

    public string playerClass;
    public bool alive;
    public int experience, money, playerLevel;
    [SerializeField]
    public int neededXP = 1000;
    private int[] xpIntervals = new int[] {1000,2000,3000,4000,5000,10000,15000,20000,30000,40000,49999};
    private int[] levelIntervals = new int[] { 10, 20, 30, 40, 50, 60, 70, 80, 90, 96, 98 };
    //10,20,30,40,50,60,70,80,90,96,98

    public void gainExperience(int xp)
    {
        int gainedXP = xp += experience;
        experience = gainedXP;
        while (experience > neededXP)
		{
            playerLevel++;
            changeNeededXP();
        }
		
        Debug.Log(experience);
    }
    
    public void changeNeededXP()
    {
        int i = 0;
        foreach (int element in levelIntervals)
        {
            if (element == playerLevel)
            {
                neededXP = neededXP + xpIntervals[i];
            }
            else
            {
                i++;   
            }
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

    //Might Change due to the way armor works
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
    
    public void damagePlayer(int damage, float armorPeircingValue, int bulletArmorPower)
    {
        if (stoppingPower > bulletArmorPower)
        {
            Debug.Log("Bullet Stopped");
        }
        else if (armorPoints > 0)
        {
            if (armorResistance >= armorPeircingValue && armorPoints > 0)
            {
                int damagedHealth = health - Mathf.RoundToInt(damage / armorResistance);
                health = damagedHealth;
            }
            else
            {
                int damagedHealth = health - damage;
                health = damagedHealth;
            }
        }
        else
        {
            int damagedHealth = health - damage;
            health = damagedHealth;
        }
        Debug.Log(health); 
        armorPoints--;
    }

    public void addPlayerMoney(int amount)
    {
    	int newAmount = money + amount;
     	money = newAmount;
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = maxHealth;
        armorPoints = maxArmorPoints;
    }
}
