using System.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class BloaterHealthSystem : AbstractHealthSystem
{
    public override HealthSystemStats Stats => stats;

    [SerializeField] HealthSystemStats stats;
    [SerializeField] ZombieStateMachine stateMachine;
    [SerializeField] NetworkObject networkObject;
    [SerializeField] SpriteRenderer zombieSprite;
    [SerializeField] ZombieBlastAttack blastAttack;
    [SerializeField] HealthEffect effect;

    //Sound Functionality
    [SerializeField] private string[] sounds;

    private float currentHealth;

    // private IEnumerator healthFlashing;
    // private IEnumerator hideHealth;
    private bool IsDead => currentHealth <= 0;
    Coroutine redFlashCoroutine;
    bool isDying;
    bool isRedFlashing;

    // Server Behaviours

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            currentHealth = stats.MaxHealth;
            isDying = false;
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
        // StopCoroutine(healthFlashing);
        // StopCoroutine(hideHealth);
        if (isRedFlashing)
            StopCoroutine(redFlashCoroutine);

        isDying = true;
        blastAttack.Attack();
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void PlayOnDamagedEffectClientRpc(float previous, float current, float max)
    {
        effect.PlayOnDamagedEffect(previous, current, max);
    }
}