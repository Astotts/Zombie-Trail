using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class ZombieThrowAttack : AbstractAttack
{
    public override float AttackTime => stats.AttackTime;
    public override float AttackAnimationTime => stats.AttackAnimationTime;

    [SerializeField] ZombieThrowAttackStats stats;
    [SerializeField] AbstractDirectionManuver direction;
    [SerializeField] SpriteRenderer bodySprite;

    Coroutine throwCoroutine;
    bool isThrowing;
    bool isOnCooldownTimer;
    Transform currentTarget;

    public override void OnNetworkSpawn()
    {
        isOnCooldownTimer = false;

        base.OnNetworkSpawn();
    }

    public override void Attack(Transform target)
    {
        StartCoroutine(AttackCooldown(stats.Cooldown));
        currentTarget = target;
        ThrowProjectileClientRpc();

    }
    IEnumerator AttackCooldown(float seconds)
    {
        isOnCooldownTimer = true;
        yield return new WaitForSeconds(seconds);
        isOnCooldownTimer = false;
    }

    [Rpc(SendTo.ClientsAndHost)]
    void ThrowProjectileClientRpc()
    {
        throwCoroutine = StartCoroutine(ThrowProjectile());
    }

    IEnumerator ThrowProjectile()
    {
        if (isThrowing)
            StopCoroutine(throwCoroutine);
        isThrowing = true;

        float attackTime = stats.AttackTime / 2;
        float elapsed = attackTime;
        while (elapsed > 0)
        {
            elapsed -= Time.deltaTime;
            Color color = Color.Lerp(Color.green, Color.white, elapsed / attackTime);
            bodySprite.color = color;
            yield return null;
        }

        if (IsServer)
            ShootProjectileAt(currentTarget);

        while (elapsed < attackTime)
        {
            elapsed += Time.deltaTime;
            Color color = Color.Lerp(Color.green, Color.white, elapsed / attackTime);
            bodySprite.color = color;
            yield return null;
        }
    }

    void ShootProjectileAt(Transform target)
    {
        NetworkObject projectile = NetworkObjectPool.Singleton.GetNetworkObject(stats.ProjectileStats.Prefab, transform.position, Quaternion.identity);

        ThrownProjectileMovement projectileMovement = projectile.GetComponent<ThrownProjectileMovement>();

        projectileMovement.InitializeInfo(stats.ProjectileStats, target.position);

        projectile.Spawn();
    }

    public override bool CanAttack(Transform target)
    {
        return !isOnCooldownTimer && Vector2.Distance(transform.position, target.position) < stats.Range;
    }
}