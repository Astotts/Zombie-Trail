using System;
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
    Camera mainCamera;
    InputAction moveAction;
    InputAction lookAction;
    InputAction spawnItemAction;
    InputAction pickUpItemAction;
    InputAction dropItemAction;

    protected override void OnCreate()
    {
        RequireForUpdate<PlayerTag>();

        moveAction = InputSystem.actions.FindAction("Move");
        lookAction = InputSystem.actions.FindAction("Look");
        spawnItemAction = InputSystem.actions.FindAction("SpawnItem");
        pickUpItemAction = InputSystem.actions.FindAction("PickUpItem");
        dropItemAction = InputSystem.actions.FindAction("DropItem");
    }

    protected override void OnStartRunning()
    {
        mainCamera = Camera.main;
    }

    protected override void OnUpdate()
    {
        float2 moveVector = moveAction.ReadValue<Vector2>();
        float2 mousePos = lookAction.ReadValue<Vector2>();
        
        foreach (
            var (moveInput, mousePositionInput, spawnItemInput, pickUpInput, dropInput)
            in
            SystemAPI
                .Query<RefRW<PlayerMoveInput>,
                    RefRW<PlayerMousePositionInput>,
                    RefRW<PlayerSpawnItemInput>,
                    RefRW<PlayerPickUpItemInput>,
                    RefRW<PlayerDropItemInput>>()
                .WithAll<GhostOwnerIsLocal>()
        )
        {
            moveInput.ValueRW.Value = moveVector;
            float3 worldPos = mainCamera.ScreenToWorldPoint(new float3(mousePos.x, mousePos.y, 0));
            mousePositionInput.ValueRW.Value = new float2(worldPos.x, worldPos.y);
            
            if (spawnItemInput.ValueRO.Value.IsSet)
                spawnItemInput.ValueRW.Value = default;
            if (spawnItemAction.WasPerformedThisFrame())
                spawnItemInput.ValueRW.Value.Set();

            if (pickUpInput.ValueRO.Value.IsSet)
                pickUpInput.ValueRW.Value = default;
            if (pickUpItemAction.WasPerformedThisFrame())
                pickUpInput.ValueRW.Value.Set();

            if (dropInput.ValueRO.Value.IsSet)
                dropInput.ValueRW.Value = default;
            if (dropItemAction.WasPerformedThisFrame())
                dropInput.ValueRW.Value.Set();
        }
    }
}
