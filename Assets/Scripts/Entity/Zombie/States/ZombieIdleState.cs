using System.Collections.Generic;
using System.ComponentModel.Design;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEditor.Tilemaps;
using UnityEngine;

public class ZombieIdleState : BaseZombieState
{
    [SerializeField] ZombieStateMachine stateMachine;
    [SerializeField] DirectionManuver directionManuver;
    [SerializeField] ZombieMovement movement;
    [SerializeField] AbstractAttack attack;
    private float searchCooldownTimer;
    private bool IsOnSearchCooldown { get { return searchCooldownTimer > 0; } }

    void OnValidate()
    {
        if (stateMachine == null)
            stateMachine = GetComponent<ZombieStateMachine>();
    }

    public override void StateUpdate()
    {
        if (IsOnSearchCooldown)
        {
            searchCooldownTimer -= Time.deltaTime;
            return;
        }

        searchCooldownTimer = directionManuver.Stats.SearchCooldown;

        if (!directionManuver.FindNearestTarget())
            return;

        if (attack.CanAttack())
            stateMachine.ChangeState(EZombieState.Attack);
        else
            stateMachine.ChangeState(EZombieState.Walk);
    }

    public override void Exit()
    {
        searchCooldownTimer = 0;
    }
}