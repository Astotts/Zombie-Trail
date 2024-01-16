using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthSystem : HealthSystem
{
    //Declaration
    public Slider healthBar;

    //Visuals
    [SerializeField] private Image sprite; //Holds Health and Health Background
    [SerializeField] private Color displayColor; //Holds Health and Health Background
    [SerializeField] private float singleFlashTime;
    [SerializeField] private float flashCycles;


    public override void Awake()
    {
        // Assigning currentHealth & healthBar to the value of maxHealth
        currentHealth = maxHealth;
        healthBar.value = maxHealth;
    }

    public override void AlterHealth(int amount)
    {
        StartCoroutine("HealthFlashing");
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
        StopCoroutine("HealthFlashing");
        // Death animation, game over screen, etc.
        Debug.LogWarning("You Are Dead.");

        //Removes gameObject
        //Destroy(gameObject);

        //!DEBUG RESET TO HEALTH DELETE LATER
        currentHealth = maxHealth;
    }

    IEnumerator HealthFlashing(){
        float elapsed = 0f;
        for(int i = 0; i <= flashCycles; i++){
            while(elapsed <= singleFlashTime){ //Turn to White
                elapsed += Time.deltaTime;
                Color color = Color.Lerp(displayColor, Color.white, (elapsed / (singleFlashTime / 2f)));
                sprite.color = color;
                yield return null;
            }
            elapsed = 0f;
            while(elapsed <= singleFlashTime){ //Turn to Health Color
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
