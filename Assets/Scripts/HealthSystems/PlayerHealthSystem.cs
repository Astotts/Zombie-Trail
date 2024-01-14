using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthSystem : HealthSystem
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
        healthBar.value = currentHealth;

        // Check for death
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public override void Die()
    {
        // Death animation, game over screen, etc.
        Debug.LogWarning("You Are Dead.");

        //Removes gameObject
        //Destroy(gameObject);

        //!DEBUG RESET TO HEALTH DELETE LATER
        currentHealth = maxHealth;
    }
}
