using System.Collections;
using System.ComponentModel;
using System.Runtime.InteropServices;
using TreeEditor;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.WSA;

public class ZombieMeleeAttack : AbstractAttack
{
    public override float AttackTime => stats.AttackTime;
    public override float AttackAnimationTime => stats.AnimationAttackTime;

    [SerializeField] SpriteRenderer hitboxDisplay;
    [SerializeField] LayerMask layerToAttack;
    [SerializeField] ZombieMeleeAttackStats stats;

    Coroutine attackCoroutine;
    float currentOpacity;
    bool isAttacking;

    #region Debug
    void OnDrawGizmos()
    {
        if (stats == null)
            return;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(stats.HitboxCenter, stats.HitboxExtends);
    }
    #endregion

    void Start()
    {
        if (IsServer)
            return;

        // this.enabled = false;
        currentOpacity = hitboxDisplay.color.a;
    }


    public override void Attack()
    {
        PlayAttackEffectClientRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    void PlayAttackEffectClientRpc()
    {
        PlayAttackEffect();
    }

    void PlayAttackEffect()
    {
        attackCoroutine = StartCoroutine(AttackEffect());
    }

    IEnumerator AttackEffect()
    {
        if (isAttacking)
            StopCoroutine(attackCoroutine);
        isAttacking = true;

        float totalAttackTime = stats.AttackTime / 2;
        float timeElapsed = 0;

        while (timeElapsed < totalAttackTime)
        {
            timeElapsed += Time.deltaTime;
            float opacity = timeElapsed / totalAttackTime * (1 - currentOpacity) + currentOpacity;
            SetOpacity(hitboxDisplay, opacity);
            yield return null;
        }

        if (IsServer)
            DealDamage();

        timeElapsed = totalAttackTime;

        while (timeElapsed > 0)
        {
            timeElapsed -= Time.deltaTime;
            float opacity = timeElapsed / totalAttackTime * (1 - currentOpacity) + currentOpacity;
            SetOpacity(hitboxDisplay, opacity);
            yield return null;
        }

        isAttacking = false;
    }

    void SetOpacity(SpriteRenderer spriteRenderer, float opacity)
    {
        Color original = spriteRenderer.color;

        Color newColor = new Color(original.r, original.g, original.b, opacity);

        spriteRenderer.color = newColor;
    }

    void DealDamage()
    {
        Vector2 hitboxRotated = transform.rotation * stats.HitboxCenter;
        Vector2 attackerPos = transform.position;

        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(attackerPos + hitboxRotated, stats.HitboxExtends, transform.eulerAngles.z, layerToAttack);
        if (hitColliders.Length == 0)
            return;

        foreach (Collider2D collider in hitColliders)
        {
            if (!collider.TryGetComponent(out IDamageable damageable))
                continue;

            damageable.Damage(stats.Damage);
        }
    }

    public override bool CanAttack()
    {
        Vector2 hitboxRotated = transform.rotation * stats.HitboxCenter;
        Vector2 attackerPos = transform.position;

        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(attackerPos + hitboxRotated, stats.HitboxExtends, transform.eulerAngles.z, layerToAttack);
        return hitColliders.Length > 0;
    }
}