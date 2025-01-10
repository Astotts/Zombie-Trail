
using UnityEngine;

public class RangedZombieWalkState : BaseZombieState
{
    [SerializeField] ZombieStateMachine stateMachine;
    [SerializeField] AbstractAttack attack;
    [SerializeField] ZombieMovement movement;
    [SerializeField] RangedZombieDirectionManuver direction;

    bool IsTargetTooClose =>
        direction.Stats is RangedDirectionManuverStats stats
        && Vector2.Distance(transform.position, direction.Target.position) < stats.DistanceToKeep;

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
        else if (IsTargetTooClose)
        {
            direction.RotateAwayFromTarget();
            movement.MoveForward();
        }
        else if (attack.CanAttack())
        {
            stateMachine.ChangeState(EZombieState.Attack);
        }
        else
        {
            direction.RotateTowardTarget();
            movement.MoveForward();
        }
    }
}