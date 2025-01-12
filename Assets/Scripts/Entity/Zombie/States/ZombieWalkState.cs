using System.IO;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class ZombieWalkState : BaseZombieState
{
    [SerializeField] ZombieStateMachine stateMachine;
    [SerializeField] AbstractAttack attack;
    [SerializeField] ZombieMovement movement;
    [SerializeField] AbstractDirectionManuver direction;
    [SerializeField] Animator animator;

    void OnValidate()
    {
        if (stateMachine == null)
            stateMachine = GetComponent<ZombieStateMachine>();
    }

    public override void Enter()
    {
        animator.SetFloat("moveSpeed", movement.MoveAnimationSpeed);
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
            Quaternion dir = direction.gameObject.transform.rotation;
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
