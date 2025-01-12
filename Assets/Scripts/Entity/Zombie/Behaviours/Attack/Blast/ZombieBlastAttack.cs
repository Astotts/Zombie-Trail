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
    [SerializeField] MeleeDirectionManuver directionManuver;
    [SerializeField] GameObject bloodSplatterGO;
    [SerializeField] ZombieBlastAttackStats stats;
    [SerializeField] Collider2D selfCollider;
    bool isAttacking;

    #region Debug
    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(stats.HitboxCenter, stats.HitboxExtends);
    }
    #endregion

    public override void Attack()
    {
        if (isAttacking)
            return;

        PlayAttackEffectClientRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    void PlayAttackEffectClientRpc()
    {
        StartCoroutine(AttackEffect());
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

        if (IsServer)
        {
            DealDamage();
            Despawn();
        }

        selfCollider.enabled = false;
        NetworkObject networkObject = NetworkObjectPool.Singleton.GetNetworkObject(bloodSplatterGO, transform.position, Quaternion.identity);
        networkObject.Spawn();
        selfCollider.enabled = true;

        isAttacking = false;
    }

    private void Despawn()
    {
        GameObject prefab = stats.Prefab;

        NetworkObjectPool.Singleton.ReturnNetworkObject(networkObject, prefab);

        networkObject.Despawn(false);
    }

    private void DealDamage()
    {
        Vector2 hitboxOrigin = (Vector2)transform.position + stats.HitboxCenter;

        selfCollider.enabled = false;
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(hitboxOrigin, stats.HitboxExtends, transform.eulerAngles.z, stats.LayerToAttack);
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

    public override bool CanAttack()
    {
        if (Vector2.Distance(directionManuver.Target.transform.position, selfCollider.transform.position) > stats.AttackRange)
            return false;

        Vector2 hitboxOrigin = (Vector2)transform.position + stats.HitboxCenter;
        selfCollider.enabled = false;
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(hitboxOrigin, stats.HitboxExtends, transform.eulerAngles.z, stats.LayerToDetect);
        selfCollider.enabled = true;
        return hitColliders.Length > 0;
    }
}