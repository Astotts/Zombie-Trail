using System;
using System.Collections;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class ZombieAttackState : BaseZombieState
{
    [SerializeField] ZombieStateMachine stateMachine;
    [SerializeField] AbstractAttack zombieAttack;
    [SerializeField] AbstractDirectionManuver directionManuver;
    [SerializeField] ZombieMovement movement;

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
        attackTimer = zombieAttack.Stats.AttackTime;
    }

    public override void StateUpdate()
    {
        if (IsAttacking)
            attackTimer -= Time.deltaTime;
        else
            stateMachine.ChangeState(EZombieState.Idle);

        directionManuver.RotateTowardTarget();
        movement.MoveForward();
    }
}