using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    //Declaration
    public int maxHealth = 100;
    private int currentHealth;
    public Slider healthBar;

    private void Awake()
    {
        // Assigning currentHealth & healthBar to the value of maxHealth
        currentHealth = maxHealth;
        healthBar.value = maxHealth;
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
