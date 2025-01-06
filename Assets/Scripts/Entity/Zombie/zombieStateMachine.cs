
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ZombieStateMachine : MonoBehaviour
{
    public ZombieAttackState AttackState { get; private set; }
    public ZombieIdleState IdleState { get; private set; }
    public ZombieWalkState WalkState { get; private set; }
    [SerializeField] TMP_Text text;
    IZombieState currentState;

    public void Init(IZombie zombie)
    {
        AttackState = new(this, zombie);
        IdleState = new(this, zombie);
        WalkState = new(this, zombie);
        currentState = IdleState;
    }

    public void ChangeState(IZombieState state)
    {
        currentState?.End();

        currentState = state;
        text.text = currentState.ToString();

        currentState.Start();
    }

    void FixedUpdate()
    {
        currentState.Update();
    }
}

public interface IZombieState
{
    void Start();
    void Update();
    void End();
}


public enum EZombieState
{
    Null,
    Idle,
    Walk,
    Attack
}
