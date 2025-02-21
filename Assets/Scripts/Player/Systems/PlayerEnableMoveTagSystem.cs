using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

// [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
[UpdateBefore(typeof(MoveAnimationSystem))]
public partial struct PlayerEnableMoveTagSystem : ISystem
{
    EntityQuery playerQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // state.RequireForUpdate<PlayerTag>();
        
        playerQuery = SystemAPI
            .QueryBuilder()
            .WithAllRW<MoveDirection>()
            .WithAll<PlayerTag, PlayerMoveInput, GhostOwner, PlayerMoveTag>()
            .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)
            .Build();

        playerQuery.AddChangedVersionFilter(ComponentType.ReadOnly<PlayerMoveTag>());
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {

        new PlayerEnableMoveTagJob
        {
            IsServer = state.World.IsServer(),
            MoveTagLookup = SystemAPI.GetComponentLookup<MovingTag>(false)
        }.ScheduleParallel(playerQuery);
    }

    [BurstCompile]
    public partial struct PlayerEnableMoveTagJob : IJobEntity
    {
        public bool IsServer;
        [NativeDisableParallelForRestriction] public ComponentLookup<MovingTag> MoveTagLookup;
        
        public void Execute(Entity playerEntity, EnabledRefRO<PlayerMoveTag> playerMoveTag, ref MoveDirection moveDirection, in PlayerMoveInput moveInput, in GhostOwner ghostOwner)
        {
            moveDirection.Value = moveInput.Value;
            MoveTagLookup.SetComponentEnabled(playerEntity, playerMoveTag.ValueRO);
        }
    }
}