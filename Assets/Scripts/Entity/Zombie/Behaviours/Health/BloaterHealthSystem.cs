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
    [SerializeField] HealthEffect effect;

    //Sound Functionality

    private float currentHealth;
    private bool IsDead => currentHealth <= 0;
    bool isDying;

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
        isDying = true;
        stateMachine.ChangeState(EZombieState.Attack);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void PlayOnDamagedEffectClientRpc(float previous, float current, float max)
    {
        effect.PlayOnDamagedEffect(previous, current, max);
    }
}