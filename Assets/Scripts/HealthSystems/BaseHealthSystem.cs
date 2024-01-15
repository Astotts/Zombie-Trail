using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseHealthSystem : HealthSystem
{
    //Declaration
    public Slider healthBar;

    public override void Awake()
    {
        // Assigning currentHealth & healthBar to the value of maxHealth
        currentHealth = maxHealth;
        healthBar.value = maxHealth;
    }

    public override void AlterHealth(int amount)
    {
        currentHealth += amount;
//<<<<<<< Updated upstream
        healthBar.value = currentHealth;
/*
=======
        healthBar.value = maxHealth;
>>>>>>> Stashed changes
        */

        // Check for death
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public override void Die()
    {
        // Death animation, game over screen, etc.
//<<<<<<< Updated upstream
        Debug.LogWarning("Your Base Has Been Destroyed.");
/*=======
        Debug.Log("The Base Has Been Destroyed.");
>>>>>>> Stashed changes */

        //Removes gameObject
        //Destroy(gameObject);

        //!DEBUG RESET TO HEALTH DELETE LATER
        currentHealth = maxHealth;
    }
}
