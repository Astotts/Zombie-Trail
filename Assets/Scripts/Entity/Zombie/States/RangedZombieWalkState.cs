
using UnityEngine;

public class RangedZombieWalkState : BaseZombieState
{
    [SerializeField] ZombieStateMachine stateMachine;
    [SerializeField] AbstractAttack attack;
    [SerializeField] ZombieMovement movement;
    [SerializeField] RangedZombieDirectionManuver direction;
    [SerializeField] Animator animator;

    Transform currentTarget;


    void OnValidate()
    {
        if (stateMachine == null)
            stateMachine = GetComponent<ZombieStateMachine>();
    }

    public override void Enter()
    {
        animator.SetFloat("moveSpeed", movement.MoveAnimationSpeed);
        currentTarget = direction.FindNearestTarget();
    }

    public override void StateUpdate()
    {
        currentTarget = direction.FindNearestTarget();
        if (currentTarget == null)
        {
            stateMachine.ChangeState(EZombieState.Idle);
        }
        else if (attack.CanAttack(currentTarget))
        {
            stateMachine.ChangeState(EZombieState.Attack);
        }
        else if (direction.IsTargetTooClose)
        {
            direction.RotateAwayFromTarget();
            movement.MoveForward();
            Quaternion dir = direction.gameObject.transform.rotation;
            Debug.DrawRay(transform.position, dir * Vector2.right);
            PlayWalkAnimation(dir.eulerAngles.z);
        }
        else
        {
            direction.RotateTowardTarget();
            movement.MoveForward();
            Quaternion dir = direction.gameObject.transform.rotation;
            Debug.DrawRay(transform.position, dir * Vector2.right);
            PlayWalkAnimation(dir.eulerAngles.z);
        }
    }

    void PlayWalkAnimation(float zRotation)
    {
        float normalizedRotation = zRotation % 360;

        if (IsRotatingUp(normalizedRotation))
        {
            animator.Play("Move Up");
        }
        else if (IsRotatingDown(normalizedRotation))
        {
            animator.Play("Move Down");
        }
        else if (IsRotatingLeft(normalizedRotation))
        {
            animator.Play("Move Left");
        }
        else if (IsRotatingRight(normalizedRotation))
        {
            animator.Play("Move Right");
        }
    }

    bool IsRotatingUp(float normalizedRotation)
    {
        return 45 < normalizedRotation && normalizedRotation < 135;
    }

    bool IsRotatingLeft(float normalizedRotation)
    {
        return 135 < normalizedRotation && normalizedRotation < 225;
    }

    bool IsRotatingDown(float normalizedRotation)
    {
        return 225 < normalizedRotation && normalizedRotation < 315;
    }

    bool IsRotatingRight(float normalizedRotation)
    {
        return 45 < normalizedRotation || normalizedRotation > 315;
    }
}