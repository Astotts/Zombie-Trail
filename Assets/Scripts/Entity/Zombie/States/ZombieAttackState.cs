using System;
using System.Collections;
using System.Data;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[Serializable]
public class ZombieAttackState : BaseZombieState
{
    [SerializeField] ZombieStateMachine stateMachine;
    [SerializeField] AbstractAttack zombieAttack;
    [SerializeField] AbstractDirectionManuver direction;
    [SerializeField] Animator animator;

    float attackTimer;
    bool IsAttacking => attackTimer > 0;

    void OnValidate()
    {
        if (stateMachine == null)
            stateMachine = GetComponent<ZombieStateMachine>();
    }

    public override void Enter()
    {
        attackTimer = zombieAttack.AttackTime;
        Transform target = direction.FindNearestTarget();
        if (target == null)
        {
            stateMachine.ChangeState(EZombieState.Idle);
            return;
        }

        zombieAttack.Attack(target);

        animator.SetFloat("attackSpeed", zombieAttack.AttackAnimationTime);
        animator.Play("Attack");
    }

    public override void StateUpdate()
    {
        if (IsAttacking)
            attackTimer -= Time.deltaTime;
        else
            stateMachine.ChangeState(EZombieState.Idle);

        direction.RotateTowardTarget();
    }
}