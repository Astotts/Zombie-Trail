using System.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class ZombieHealthSystem : AbstractHealthSystem
{
    public override HealthSystemStats Stats => stats;

    [SerializeField] HealthSystemStats stats;
    [SerializeField] NetworkObject networkObject;
    [SerializeField] HealthEffect effect;

    bool isDying;

    private float currentHealth;

    // private IEnumerator healthFlashing;
    // private IEnumerator hideHealth;
    private bool IsDead => currentHealth <= 0;

    Coroutine redFlashCoroutine;
    bool isRedFlashing;

    // Server Behaviours

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            isDying = false;
            currentHealth = stats.MaxHealth;
        }

        base.OnNetworkSpawn();
    }

    public override void Damage(float amount)
    {
        float previous = currentHealth;
        currentHealth -= amount;
        PlayOnDamagedEffectClientRpc(previous, currentHealth, stats.MaxHealth);

        if (IsDead && !isDying)
            Die();
    }

    public void Die()
    {
        isDying = true;
        // StopCoroutine(healthFlashing);
        // StopCoroutine(hideHealth);
        if (isRedFlashing)
            StopCoroutine(redFlashCoroutine);

        networkObject.Despawn();
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void PlayOnDamagedEffectClientRpc(float previous, float current, float max)
    {
        effect.PlayOnDamagedEffect(previous, current, max);
    }
}