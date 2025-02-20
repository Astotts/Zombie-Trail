using System;
using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Netcode;
using Unity.Services.Relay.Scheduler;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

public class ZombieBlastAttack : AbstractAttack
{
    public override float AttackTime => stats.AttackTime;
    public override float AttackAnimationTime => stats.AnimationAttackTime;

    [SerializeField] SpriteRenderer bloaterSprite;
    [SerializeField] NetworkObject networkObject;
    [SerializeField] ZombieBlastAttackStats stats;
    [SerializeField] Collider2D selfCollider;
    Coroutine explodeCoroutine;
    bool isAttacking;

    #region Debug
    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube((Vector2)transform.position + stats.HitboxCenter, stats.HitboxExtends);
    }
    #endregion

    public override void OnNetworkSpawn()
    {
        isAttacking = false;
    }

    public override void OnNetworkDespawn()
    {
        if (isAttacking)
            StopCoroutine(explodeCoroutine);

        base.OnNetworkDespawn();
    }

    public override void Attack(Transform target)
    {
        PlayAttackEffectClientRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    void PlayAttackEffectClientRpc()
    {
        explodeCoroutine = StartCoroutine(AttackEffect());
    }

    IEnumerator AttackEffect()
    {
        isAttacking = true;

        float totalAttackTime = stats.AttackTime;
        float timeElapsed = 0;

        while (timeElapsed < totalAttackTime)
        {
            timeElapsed += Time.deltaTime;
            float step = timeElapsed / totalAttackTime;
            Color color = Color.Lerp(Color.white, Color.red, step);
            bloaterSprite.color = color;
            yield return null;
        }

        selfCollider.enabled = false;
        NetworkObject networkObject = NetworkObjectPool.Singleton.GetNetworkObject(stats.ParticleOnBlast, transform.position, Quaternion.identity);
        networkObject.GetComponent<ParticleSystem>().Play();
        networkObject.Spawn();
        selfCollider.enabled = true;

        if (IsServer)
        {
            DealDamage();
            Despawn();
        }

        isAttacking = false;
    }

    private void Despawn()
    {
        networkObject.Despawn();
    }

    private void DealDamage()
    {
        Vector2 hitboxOrigin = (Vector2)transform.position + stats.HitboxCenter;

        selfCollider.enabled = false;
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(hitboxOrigin, stats.HitboxExtends, 0, stats.LayerToAttack);
        selfCollider.enabled = true;
        if (hitColliders.Length == 0)
            return;

        foreach (Collider2D collider in hitColliders)
        {
            if (collider.TryGetComponent(out IDamageable damageable))
                damageable.Damage(stats.Damage);

            if (collider.TryGetComponent(out IKnockable knockable))
            {
                Vector2 forceDirection = (Vector2)collider.transform.position - hitboxOrigin;

                Vector2 forceVector = forceDirection.normalized * stats.Force;

                knockable.Knock(forceVector);
            }
        }
    }

    public override bool CanAttack(Transform target)
    {
        if (Vector2.Distance(target.position, transform.position) > stats.AttackRange)
            return false;

        Vector2 hitboxOrigin = (Vector2)transform.position + stats.HitboxCenter;
        selfCollider.enabled = false;
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(hitboxOrigin, stats.HitboxExtends, transform.eulerAngles.z, stats.LayerToDetect);
        selfCollider.enabled = true;
        return hitColliders.Length > 0;
    }
}