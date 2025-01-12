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
    float cooldownTimer;

    public override void Attack()
    {
        cooldownTimer = stats.Cooldown;
        throwCoroutine = StartCoroutine(ThrowProjectile());
    }

    void FixedUpdate()
    {
        if (cooldownTimer > 0)
            cooldownTimer -= Time.fixedDeltaTime;
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

        SpawnProjectile(direction.Target);

        while (elapsed < attackTime)
        {
            elapsed += Time.deltaTime;
            Color color = Color.Lerp(Color.green, Color.white, elapsed / attackTime);
            bodySprite.color = color;
            yield return null;
        }
    }

    void SpawnProjectile(Transform target)
    {
        NetworkObject projectile = NetworkObjectPool.Singleton.GetNetworkObject(stats.ProjectileStats.Prefab, transform.position, Quaternion.identity);

        ThrownProjectileMovement projectileMovement = projectile.GetComponent<ThrownProjectileMovement>();

        projectileMovement.InitializeInfo(stats.ProjectileStats, target.position);

        projectile.Spawn();
    }

    public override bool CanAttack()
    {
        return cooldownTimer <= 0 && Vector2.Distance(transform.position, direction.Target.position) < stats.Range;
    }
}