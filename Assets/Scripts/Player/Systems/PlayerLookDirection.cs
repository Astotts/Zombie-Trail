using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
[UpdateAfter(typeof(PredictedSimulationSystemGroup))]
public partial struct PlayerLookDirectionSystem : ISystem
{
    EntityQuery playerQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerTag>();

        playerQuery = SystemAPI
            .QueryBuilder()
            .WithAllRW<LookDirection>()
            .WithAll<PlayerLookInput, LocalTransform>()
            .Build();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        new PlayerLookDirectionJob().ScheduleParallel(playerQuery);
    }

    [BurstCompile]
    public partial struct PlayerLookDirectionJob : IJobEntity
    {
        public void Execute(ref LookDirection lookDirection, in PlayerLookInput lookInput, in LocalTransform playerTransform)
        {
            float2 worldPos = lookInput.WorldPos;
            float3 playerPos = playerTransform.Position;
            
            lookDirection.Value = new(worldPos.x - playerPos.x, worldPos.y - playerPos.y);
        }
    }
}