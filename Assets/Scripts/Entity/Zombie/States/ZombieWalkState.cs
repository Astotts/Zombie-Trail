using UnityEngine;

public class ZombieWalkState : IZombieState
{
    readonly ZombieStateMachine stateMachine;
    readonly IZombie zombie;
    public ZombieWalkState(ZombieStateMachine stateMachine, IZombie zombie)
    {
        this.stateMachine = stateMachine;
        this.zombie = zombie;
    }

    public void Start()
    {
        
    }

    public void Update()
    {
        if (zombie.MoveTowardTarget())
            return;

        stateMachine.ChangeState(stateMachine.IdleState);
    }

    public void End()
    {
        
    }
}
