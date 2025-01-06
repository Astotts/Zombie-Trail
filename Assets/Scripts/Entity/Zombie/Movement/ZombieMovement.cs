using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieMovement : MonoBehaviour
{
    [SerializeField] Transform eyeDirection;
    [SerializeField] Rigidbody2D rb2D;
    IZombie zombie;

    public void Init(IZombie zombie)
    {
        this.zombie = zombie;
    }

    public bool MoveTowardTarget(Transform target)
    {
        float distanceToTarget = Vector2.Distance(target.position, transform.position);
        if (distanceToTarget < zombie.Stats.AttackRange)
            return false;

        Vector2 direction = eyeDirection.rotation * Vector2.right;
        rb2D.MovePosition(rb2D.position + zombie.Stats.MoveSpeed * Time.deltaTime * direction);
        return true;
    }
}
