using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class CarHealth : NetworkBehaviour, IDamageable
{
    [SerializeField] int healthFlashCycles = 4;
    [SerializeField] float healthSingleFlashTime = 0.25f;
    [SerializeField] CarStats _carStats;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] NetworkObject networkObject;

    public CarStats Stats => _carStats;

    private NetworkVariable<float> currentHealth = new();
    private bool isDestroyed => currentHealth.Value <= 0;
    private Coroutine healthFlashing;

    void Start()
    {
        if (!IsHost)
            return;

        currentHealth.Value = Stats.Health;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        currentHealth.OnValueChanged += PlayOnDamagedEffect;
    }

    void PlayOnDamagedEffect(float previousHealth, float currentHealth)
    {
        healthFlashing = StartCoroutine(HealthFlashing());
    }

    // Only host is allowed to use this
    public void Damage(float amount)
    {
        currentHealth.Value -= amount;

        if (isDestroyed)
        {
            DestroyVehicle();
        }
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

    void DestroyVehicle()
    {
        StopCoroutine(healthFlashing);
        networkObject.Despawn();
    }
}