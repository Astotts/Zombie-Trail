using System.IO;
using UnityEngine;

public class ZombieWalkState : BaseZombieState
{
    [SerializeField] ZombieStateMachine stateMachine;
    [SerializeField] AbstractAttack attack;
    [SerializeField] ZombieMovement movement;
    [SerializeField] DirectionManuver direction;

    void OnValidate()
    {
        if (stateMachine == null)
            stateMachine = GetComponent<ZombieStateMachine>();
    }

    public override void StateUpdate()
    {
        if (direction.Target == null)
        {
            stateMachine.ChangeState(EZombieState.Idle);
        }
        else if (attack.CanAttack())
        {
            stateMachine.ChangeState(EZombieState.Attack);
        }
        else
        {
            movement.MoveForward();
            direction.RotateTowardTarget();
        }
    }
}
