using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[UpdateInGroup(typeof(GhostInputSystemGroup))]
public partial class PlayerInputSystem : SystemBase
{
    InputAction moveAction;
    InputAction lookAction;

    protected override void OnCreate()
    {
        RequireForUpdate<PlayerTag>();

        moveAction = InputSystem.actions.FindAction("Move");
        lookAction = InputSystem.actions.FindAction("Look");
    }

    protected override void OnUpdate()
    {
        float2 moveVector = moveAction.ReadValue<Vector2>();
        float2 mousePos = lookAction.ReadValue<Vector2>();
        
        foreach (
            var (moveInput, lookInput)
            in
            SystemAPI
                .Query<RefRW<PlayerMoveInput>, RefRW<PlayerLookInput>>()
                .WithAll<GhostOwnerIsLocal>()
        )
        {   
            moveInput.ValueRW.Value = moveVector;
            lookInput.ValueRW.ScreenPos = mousePos;
            float3 worldPos = Camera.main.ScreenToWorldPoint(new float3(mousePos.x, mousePos.y, 0));
            lookInput.ValueRW.WorldPos = new float2(worldPos.x, worldPos.y);
        }
    }
}
