using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloaterAttackState : BaseZombieState
{
    [SerializeField] ZombieStateMachine stateMachine;
    [SerializeField] AbstractAttack zombieAttack;
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
        zombieAttack.Attack();
        attackTimer = zombieAttack.AttackTime;

        animator.Play("Attack");
    }

    public override void StateUpdate()
    {
        if (IsAttacking)
            attackTimer -= Time.deltaTime;
        else
            stateMachine.ChangeState(EZombieState.Idle);
    }
}
