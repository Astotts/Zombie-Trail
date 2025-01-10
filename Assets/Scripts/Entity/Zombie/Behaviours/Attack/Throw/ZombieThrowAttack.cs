using System.Collections;
using System.Diagnostics;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ZombieThrowAttack : AbstractAttack
{
    public override AbstractAttackStats Stats => stats;

    [SerializeField] ZombieThrowAttackStats stats;
    [SerializeField] DirectionManuver direction;
    [SerializeField] SpriteRenderer bodySprite;

    Coroutine throwCoroutine;
    bool isThrowing;

    public override void Attack()
    {
        throwCoroutine = StartCoroutine(ThrowProjectile());
    }

    IEnumerator ThrowProjectile()
    {
        if (isThrowing)
            StopCoroutine(throwCoroutine);
        isThrowing = true;

        float attackTime = stats.AttackTime;
        float elapsed = attackTime;
        while (elapsed > 0)
        {
            elapsed -= Time.deltaTime;
            Color color = Color.Lerp(Color.white, Color.green, elapsed / attackTime);
            bodySprite.color = color;
            yield return null;
        }

        SpawnProjectile(direction.Target);

        while (elapsed < attackTime)
        {
            elapsed += Time.deltaTime;
            Color color = Color.Lerp(Color.white, Color.green, elapsed / attackTime);
            bodySprite.color = color;
            yield return null;
        }
    }

    void SpawnProjectile(Transform target)
    {
        NetworkObject projectile = NetworkObjectPool.Singleton.GetNetworkObject(stats.ProjectilePrefab, transform.position, Quaternion.identity);

        ThrownProjectileMovement projectileMovement = projectile.GetComponent<ThrownProjectileMovement>();

        projectileMovement.InitializeInfo(stats.projectileStats, target.position);

        projectile.Spawn();
    }

    public override bool CanAttack()
    {
        return Vector2.Distance(transform.position, direction.Target.position) < stats.Range;
    }
}