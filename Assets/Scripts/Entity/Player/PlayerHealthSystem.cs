using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class PlayerHealth : NetworkBehaviour, IDamageable
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
    [SerializeField] NetworkObject networkObject;

    //Sound System
    [SerializeField] private string[] sounds;

    //Variables
    [SerializeField] private float maxHealth = 10;
    private readonly NetworkVariable<float> currentHealth = new();

    private Coroutine healthFlashing;

    private bool isDead => currentHealth.Value <= 0;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsHost)
            currentHealth.Value = maxHealth;

        currentHealth.OnValueChanged += PlayOnDamagedEffect;
    }

    void PlayOnDamagedEffect(float preivous, float current)
    {
        healthFlashing = StartCoroutine(HealthFlashing());
        StopCoroutine(healthFlashing);

        for (int i = 0; bloodEffect.Length > i; i++)
        {
            float alphaValue = -((current - maxHealth) / maxHealth);
            bloodEffect[i].color = new Color(bloodEffectColor.r, bloodEffectColor.g, bloodEffectColor.b, alphaValue);
        }

        StartCoroutine(HealthFlashing());
        StartCoroutine(ScreenEffect());

        healthBar.value = (maxHealth - current) / maxHealth * 100f;
        AudioManager.Instance.PlaySFX(sounds[Random.Range(0, 3)], Random.Range(0.7f, 1.1f));
    }

    void Awake()
    {
        // Assigning currentHealth & healthBar to the value of maxHealth
        // currentHealth = maxHealth;
        healthBar.value = maxHealth;
    }

    // public override void AlterHealth(int amount)
    // {
    //     if (!networkObject.IsSpawned)
    //         return;
    //     StopCoroutine("ScreenEffect");
    //     //Debug.Log(-(((float)currentHealth - (float)maxHealth) / (float)maxHealth));
    //     for (int i = 0; bloodEffect.Length > i; i++)
    //     {
    //         bloodEffect[i].color = new Color(bloodEffectColor.r, bloodEffectColor.g, bloodEffectColor.b, -(((float)currentHealth.Value - (float)maxHealth) / (float)maxHealth));
    //     }

    //     StartCoroutine("HealthFlashing");
    //     StartCoroutine("ScreenEffect");
    //     AlterHealthRpc(amount);
    //     healthBar.value = (float)currentHealth.Value / (float)maxHealth * 100f;
    //     AudioManager.Instance.PlaySFX(sounds[UnityEngine.Random.Range(0, 3)], UnityEngine.Random.Range(0.7f, 1.1f));
    // }

    // Only host is allowed to use it
    public void Damage(float amount)
    {
        currentHealth.Value -= amount;
        if (isDead)
            Die();
    }

    public void Die()
    {
        StopCoroutine(healthFlashing);
        // Death animation, game over screen, etc.
        Debug.LogWarning("You Are Dead.");

        GameObject spawnPoint = GameObject.FindWithTag("PlayerSpawn");
        transform.position = spawnPoint.transform.position;

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
