using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthSystem : HealthSystem
{
    //Declaration
    public Slider healthBar;

    //Health Visuals
    [SerializeField] private Image sprite; //Holds Health and Health Background
    [SerializeField] private Color displayColor; //Holds Health and Health Background
    [SerializeField] private float singleFlashTime;
    [SerializeField] private float flashCycles;

    //Screen Visuals
    [SerializeField] private Image[] bloodEffect;
    [SerializeField] private Color bloodEffectColor;
    [SerializeField] private float waitForFade;
    [SerializeField] private float timeToFade;


    public override void Awake()
    {

        // Assigning currentHealth & healthBar to the value of maxHealth
        currentHealth = maxHealth;
        healthBar.value = maxHealth;
    }

    public override void AlterHealth(int amount)
    {
        StopCoroutine("ScreenEffect");
        //Debug.Log(-(((float)currentHealth - (float)maxHealth) / (float)maxHealth));
        for (int i = 0; bloodEffect.Length > i; i++)
        {
            bloodEffect[i].color = new Color(bloodEffectColor.r, bloodEffectColor.g, bloodEffectColor.b, -(((float)currentHealth - (float)maxHealth) / (float)maxHealth));
        }

        StartCoroutine("HealthFlashing");
        StartCoroutine("ScreenEffect");
        currentHealth += amount;
        healthBar.value = (float)currentHealth / (float)maxHealth * 100f;

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

    IEnumerator HealthFlashing()
    {
        float elapsed = 0f;
        for (int i = 0; i <= flashCycles; i++)
        {
            while (elapsed <= singleFlashTime)
            { //Turn to White
                elapsed += Time.deltaTime;
                Color color = Color.Lerp(displayColor, Color.white, (elapsed / (singleFlashTime / 2f)));
                // sprite.color = color;
                yield return null;
            }
            elapsed = 0f;
            while (elapsed <= singleFlashTime)
            { //Turn to Health Color
                elapsed += Time.deltaTime;
                Color color = Color.Lerp(Color.white, displayColor, (elapsed / (singleFlashTime / 2f)));
                // sprite.color = color;
                yield return null;
            }
            elapsed = 0f;
        }

        yield break;
    }

    IEnumerator ScreenEffect()
    {
        float elapsed = 0f;
        while (elapsed <= waitForFade)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        elapsed = 0f;
        Color startingColor = bloodEffect[0].color;
        while (elapsed <= timeToFade)
        {
            elapsed += Time.deltaTime;
            for (int i = 0; bloodEffect.Length > i; i++)
            {
                bloodEffect[i].color = Color.Lerp(startingColor, Color.clear, elapsed / timeToFade);
            }
            yield return null;
        }
        yield break;
    }
}
