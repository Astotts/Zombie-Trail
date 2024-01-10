using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    //Variable Declaration
    public int maxHealth = 100;
    private int currentHealth;
    public Slider healthBar;

    private void Awake()
    {
        // Assigning currentHealth a value
        currentHealth = maxHealth;
    }

    void Update()
    {
        // Update the health bar UI
        healthBar.value = currentHealth;
    }

    public void AlterHealth(int amount)
    {
        currentHealth += amount;

        // Check for death
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Death animation, game over screen, etc.
        Debug.Log("You Are Dead.");

        //Removes gameObject
        Destroy(gameObject);
    }
}
