using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.ShaderGraph;
using UnityEngine;
public class ThrownProjectileMovement : NetworkBehaviour
{
    [SerializeField] NetworkObject networkObject;
    [SerializeField] ParticleSystem orbParticle;
    [SerializeField] ParticleSystem splashParticle;

    [SerializeField] bool debug;
    [SerializeField] Vector2 debugHitboxCenter;
    [SerializeField] Vector2 debugHitboxExtends;

    ThrownProjectileStats stats;
    private float maxRelativeHeight;
    Vector2 targetPos;
    Vector2 initialPos;

    void OnDrawGizmos()
    {
        if (debug)
            Gizmos.DrawWireCube(debugHitboxCenter, debugHitboxExtends);

        if (stats == null)
            return;
        Gizmos.DrawWireCube(targetPos + stats.HitboxOrigin, stats.HitboxExtends);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsServer)
        {
            enabled = false;
            return;
        }
        StartCoroutine(FlyAnimation());
    }

    private IEnumerator FlyAnimation()
    {
        float distance = Vector2.Distance(targetPos, initialPos);
        float timeToReach = distance / stats.MaxSpeed;
        float elapsed = 0;
        while (elapsed < timeToReach)
        {
            elapsed += Time.deltaTime;
            float time = elapsed / timeToReach;
            float height = GetHeight(time);
            Vector2 newPos = Vector2.Lerp(initialPos, targetPos, time);
            transform.position = new(newPos.x, newPos.y + height);

            yield return null;
        }

        OnReach();
    }

    float GetHeight(float time)
    {
        return stats.MovementCurve.Evaluate(time) * maxRelativeHeight;
    }

    void OnReach()
    {
        transform.position = targetPos;
        PlayExplosionVFXClientRpc();
        DealDamage();
    }

    [Rpc(SendTo.ClientsAndHost)]
    void PlayExplosionVFXClientRpc()
    {
        orbParticle.Stop();

        splashParticle.Play();
    }

    void DealDamage()
    {
        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(targetPos + stats.HitboxOrigin, stats.HitboxExtends, 0, stats.LayerToAttack);

        foreach (Collider2D collider2D in collider2Ds)
        {
            if (collider2D.TryGetComponent(out IDamageable damageable))
            {
                damageable.Damage(stats.Damage);
            }

            if (collider2D.TryGetComponent(out IKnockable knockable))
            {
                Vector2 directionToTarget = (Vector2)collider2D.transform.position - targetPos;
                Vector2 forceVector = directionToTarget.normalized * stats.Force;

                knockable.Knock(forceVector);
            }
        }
    }

    public void InitializeInfo(ThrownProjectileStats stats, Vector2 targetPos)
    {
        this.stats = stats;
        this.targetPos = targetPos;
        initialPos = transform.position;

        float distanceToTarget = Vector2.Distance(initialPos, targetPos);
        maxRelativeHeight = Mathf.Abs(distanceToTarget) * stats.MaxHeight;
    }
}
