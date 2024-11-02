using System.Collections;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Car))]
public class CarHealth : NetworkBehaviour, IDamageable
{
    [SerializeField] int healthFlashCycles = 4;
    [SerializeField] float healthSingleFlashTime = 0.25f;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] NetworkObject networkObject;

    int maxHealth;

    private readonly NetworkVariable<float> currentHealth = new();
    private bool IsDestroyed => currentHealth.Value <= 0;
    private IEnumerator healthFlashing;

    void Start()
    {
        if (!IsHost)
            return;

        healthFlashing = HealthFlashing();
        currentHealth.Value = maxHealth;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        currentHealth.OnValueChanged += PlayOnDamagedEffect;
    }

    void PlayOnDamagedEffect(float previousHealth, float currentHealth)
    {
        StartCoroutine(HealthFlashing());
    }

    // Only host is allowed to use this
    public void Damage(float amount)
    {
        currentHealth.Value -= amount;

        if (IsDestroyed)
        {
            DestroyVehicle();
        }
    }

    void DestroyVehicle()
    {
        StopCoroutine(healthFlashing);
        networkObject.Despawn();
    }

    IEnumerator HealthFlashing()
    {
        float elapsed = 0f;
        Color displayColor = spriteRenderer.color;
        for (int i = 0; i <= healthFlashCycles; i++)
        {
            while (elapsed <= healthSingleFlashTime)
            { //Turn to White
                elapsed += Time.deltaTime;
                Color color = Color.Lerp(displayColor, Color.white, (elapsed / (healthSingleFlashTime / 2f)));
                spriteRenderer.color = color;
                yield return null;
            }
            elapsed = 0f;
            while (elapsed <= healthSingleFlashTime)
            { //Turn to Health Color
                elapsed += Time.deltaTime;
                Color color = Color.Lerp(Color.white, displayColor, (elapsed / (healthSingleFlashTime / 2f)));
                spriteRenderer.color = color;
                yield return null;
            }
            elapsed = 0f;
        }
    }

    public void SetMaxHealth(int amount)
    {
        maxHealth = amount;
    }
}