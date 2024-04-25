using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class BaseHealthSystem : HealthSystem
{
    //Declaration
    public Slider healthBar;

    //Visuals
    [SerializeField] private Image sprite; //Holds Health and Health Background
    [SerializeField] private Color displayColor; //Holds Health and Health Background
    [SerializeField] private float singleFlashTime;
    [SerializeField] private float flashCycles;


    public override void Start()
    {
        // Assigning currentHealth.Value & healthBar to the value of maxHealth
        currentHealth.Value = maxHealth;
        healthBar.value = maxHealth;
    }

    [Rpc(SendTo.Server)]
    public override void AlterHealthServerRpc(int amount)
    {
        StartCoroutine("HealthFlashing");
        currentHealth.Value += amount;
        healthBar.value = (float)currentHealth.Value / (float)maxHealth * 100f;

        // Check for death
        if (currentHealth.Value <= 0)
        {
            Die();
        }
    }

    public override void Die()
    {
        StopCoroutine("HealthFlashing");
        // Death animation, game over screen, etc.
        Debug.LogWarning("Your Base Has Been Destroyed.");

        //Removes gameObject
        //Destroy(gameObject);

        //!DEBUG RESET TO HEALTH DELETE LATER
        currentHealth.Value = maxHealth;
    }

    IEnumerator HealthFlashing()
    {
        float elapsed = 0f;
        for (int i = 0; i <= flashCycles; i++)
        {
            while (elapsed <= singleFlashTime)
            { //Turn to White
                elapsed += Time.deltaTime;
                Color color = Color.Lerp(displayColor, Color.white, (elapsed / (singleFlashTime / 2f)));
                sprite.color = color;
                yield return null;
            }
            elapsed = 0f;
            while (elapsed <= singleFlashTime)
            { //Turn to Health Color
                elapsed += Time.deltaTime;
                Color color = Color.Lerp(Color.white, displayColor, (elapsed / (singleFlashTime / 2f)));
                sprite.color = color;
                yield return null;
            }
            elapsed = 0f;
        }

        yield break;
    }
}