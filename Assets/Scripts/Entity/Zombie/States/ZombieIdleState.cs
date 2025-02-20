using System.Collections.Generic;
using System.ComponentModel.Design;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEditor.Tilemaps;
using UnityEngine;

public class ZombieIdleState : BaseZombieState
{
    [SerializeField] ZombieStateMachine stateMachine;
    [SerializeField] AbstractDirectionManuver directionManuver;
    [SerializeField] ZombieMovement movement;
    [SerializeField] AbstractAttack attack;

    void OnValidate()
    {
        if (stateMachine == null)
            stateMachine = GetComponent<ZombieStateMachine>();
    }

    public override void StateUpdate()
    {
        Transform target = directionManuver.FindNearestTarget();
        if (target == null)
            return;

        if (attack.CanAttack(target))
            stateMachine.ChangeState(EZombieState.Attack);
        else
            stateMachine.ChangeState(EZombieState.Walk);
    }
}