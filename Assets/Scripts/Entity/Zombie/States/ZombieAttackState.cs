using System;
using System.Collections;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class ZombieAttackState : IZombieState
{
    readonly ZombieStateMachine stateMachine;
    readonly IZombie zombie;

    float attackTimer;
    bool isAttacking => attackTimer > 0;

    public ZombieAttackState(ZombieStateMachine stateMachine, IZombie zombie)
    {
        this.stateMachine = stateMachine;
        this.zombie = zombie;
    }

    public void Start()
    {
        zombie.Attack();
        attackTimer = 1 / zombie.Stats.AttackSpeed;
    }

    public void Update()
    {
        if (isAttacking)
            attackTimer -= Time.deltaTime;
        else
            stateMachine.ChangeState(stateMachine.IdleState);
    }

    public void End()
    {
    }
}