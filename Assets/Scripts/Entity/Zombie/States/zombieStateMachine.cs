
using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class ZombieStateMachine : NetworkBehaviour
{
    [SerializeField] BaseZombieState idleState;
    [SerializeField] BaseZombieState attackState;
    [SerializeField] BaseZombieState walkState;

    BaseZombieState currentState;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsServer)
            enabled = false;

        currentState = idleState;
    }

    public void ChangeState(EZombieState state)
    {
        if (currentState != null)
            currentState.Exit();

        currentState = GetState(state);

        currentState.Enter();
    }

    BaseZombieState GetState(EZombieState type)
    {
        return type switch
        {
            EZombieState.Idle => idleState,
            EZombieState.Attack => attackState,
            EZombieState.Walk => walkState,
            _ => null,
        };
    }

    void FixedUpdate()
    {
        currentState.StateUpdate();
    }
}

public abstract class BaseZombieState : MonoBehaviour
{
    public virtual void Enter() { }
    public virtual void StateUpdate() { }
    public virtual void Exit() { }
}


public enum EZombieState
{
    Idle,
    Walk,
    Attack
}
