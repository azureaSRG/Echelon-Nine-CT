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
    public bool alive;
    public int experience;
    public int playerLevel;
    [SerializeField]
    protected int neededXP;
    private int[] xpIntervals = new int[] {1000,2000,3000,4000,5000,10000,15000,20000,30000,40000,49999};
    //10,20,30,40,50,60,70,80,90,96,98

    public void gainExperience(int xp)
    {
        int gainedXP = xp += experience;
        experience = gainedXP;
        checkLevelUp();
    }
    
    public void checkLevelUp()
    {
        if (experience >= neededXP)
        {
            playerLevel++;
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

    public void damagePlayer(int damage)
    {
        int damagedHealth = health -= damage;
        health = damagedHealth;
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
