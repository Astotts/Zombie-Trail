using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;


// [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
// [UpdateAfter(typeof(PredictedSimulationSystemGroup))]
// public partial struct PlayerLookDirectionSystem : ISystem
// {
//     EntityQuery playerQuery;

//     [BurstCompile]
//     public void OnCreate(ref SystemState state)
//     {
//         state.RequireForUpdate<PlayerTag>();

//         playerQuery = SystemAPI
//             .QueryBuilder()
//             .WithAllRW<LookDirection>()
//             .WithAll<PlayerMoveInput, LocalTransform>()
//             .Build();
//     }

//     [BurstCompile]
//     public void OnUpdate(ref SystemState state)
//     {
//         new PlayerLookDirectionJob().ScheduleParallel(playerQuery);
//     }

//     [BurstCompile]
//     public partial struct PlayerLookDirectionJob : IJobEntity
//     {
//         public void Execute(ref LookDirection lookDirection, in PlayerMoveInput moveInput, in LocalTransform playerTransform, [EntityIndexInQuery] int index)
//         {
//             // float2 worldPos = moveInput.WorldPos;
//             // float3 playerPos = playerTransform.Position;
            
//             // lookDirection.Value = new(worldPos.x - playerPos.x, worldPos.y - playerPos.y);
//             // Debug.Log($"{index} moveInput: {moveInput.Value}");
//             // if (moveInput.Value.x == 0 && moveInput.Value.y == 0)

//         }
//     }
// }