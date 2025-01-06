using System.Collections;
using System.ComponentModel;
using TreeEditor;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.WSA;

public class ZombieMeleeAttack : NetworkBehaviour
{
    public static readonly float ROTATE_SPEED = 50f;
    [SerializeField] SpriteRenderer hitboxDisplay;
    [SerializeField] LayerMask layerToAttack;
    Coroutine attackCoroutine;
    IZombie zombie;
    float currentOpacity;

    bool isAttacking;

    #region Debug
    void OnDrawGizmos()
    {
        if (zombie == null || zombie.Stats == null)
            return;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(zombie.Stats.HitboxCenter, zombie.Stats.HitboxExtends);
    }
    #endregion

    public void Init(IZombie zombie)
    {
        this.zombie = zombie;
    }

    void Start()
    {
        if (IsServer)
            return;

        // this.enabled = false;
        currentOpacity = hitboxDisplay.color.a;
    }

    void Update()
    {
        Vector2 targetPos = zombie.Target.position;

        // Finding the closest player? Look like a working as a magnet
        // target = targetFinder.GetClosest();
        // moveDirection = (Vector2)target.position - (Vector2)characterPos.position;
        // //Debug.DrawRay(transform.position, targetWorldPos, Color.red, 0.01f);

        // vector from this object towards the target location
        Vector2 playerToTargetVector = targetPos - (Vector2)transform.position;

        // rotate that vector by 90 degrees around the Z axis
        Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90) * playerToTargetVector;

        float singleStep = zombie.Stats.AttackSpeed * Time.deltaTime * ROTATE_SPEED;

        // get the rotation that points the Z axis forward, and the Y axis 90 degrees away from the target
        // (resulting in the X axis facing the target)
        Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, singleStep);
    }

    public void Attack()
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

        float attackTime = zombie.Stats.AttackTime;
        float attackSpeed = zombie.Stats.AttackSpeed;

        float totalAttackTime = attackTime / attackSpeed;
        float timeElapsed = 0;

        while (timeElapsed < totalAttackTime)
        {
            timeElapsed += Time.deltaTime;
            float opacity = timeElapsed / totalAttackTime * (1 - currentOpacity) + currentOpacity;
            hitboxDisplay.color = new Color(1, 1, 1, opacity);
            yield return null;
        }

        DealDamage();

        totalAttackTime = attackTime / attackSpeed;
        timeElapsed = totalAttackTime;

        while (timeElapsed > 0)
        {
            timeElapsed -= Time.deltaTime;
            float opacity = timeElapsed / totalAttackTime * (1 - currentOpacity) + currentOpacity;
            hitboxDisplay.color = new Color(1, 1, 1, opacity);
            yield return null;
        }

        isAttacking = false;
    }

    void DealDamage()
    {
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(transform.rotation * zombie.Stats.HitboxCenter, zombie.Stats.HitboxExtends, transform.eulerAngles.z, layerToAttack);
        if (hitColliders.Length == 0)
            return;

        foreach (Collider2D collider in hitColliders)
        {
            if (!collider.TryGetComponent(out IDamageable damageable))
                continue;

            damageable.Damage(zombie.Stats.Damage);
        }
    }
}