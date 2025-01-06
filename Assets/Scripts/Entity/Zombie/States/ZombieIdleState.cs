using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEditor.Search;
using UnityEditor.Tilemaps;
using UnityEngine;

public class ZombieIdleState : IZombieState
{
    readonly ZombieStateMachine stateMachine;
    readonly IZombie zombie;

    private float searchCooldownTimer;

    private bool IsOnSearchCooldown { get { return searchCooldownTimer > 0; } }

    public ZombieIdleState(ZombieStateMachine stateMachine, IZombie zombie)
    {
        this.zombie = zombie;
        this.stateMachine = stateMachine;
    }

    public void Start()
    {

    }

    public void Update()
    {
        if (IsOnSearchCooldown)
        {
            searchCooldownTimer -= Time.deltaTime;
            return;
        }

        searchCooldownTimer = zombie.Stats.SearchInterval;

        if (zombie.CanAttack)
        {
            stateMachine.ChangeState(stateMachine.AttackState);
        }
        else if (zombie.FindTarget())
        {
            stateMachine.ChangeState(stateMachine.WalkState);
        }
    }

    public void End()
    {
    }
}