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
    protected bool alive;
    [SerializeField]
    protected int experience;
    [SerializeField]
    protected int playerLevel;

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
