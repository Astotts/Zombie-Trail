using Unity.Burst;
using Unity.Entities;
using UnityEngine;

// [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
// public partial struct PlayerMoveDirectionSystem : ISystem
// {
//     [BurstCompile]
//     public void OnCreate(ref SystemState state)
//     {
//         state.RequireForUpdate<PlayerTag>();
//     }

//     [BurstCompile]
//     public void OnUpdate(ref SystemState state)
//     {
//         int x = 0;
//         foreach (
//             var (moveDirection, moveInput)
//             in
//             SystemAPI
//                 .Query<RefRW<MoveDirection>, RefRO<PlayerMoveInput>>()
//         )
//         {
//             Debug.Log($"{x} moveinput: {moveInput.ValueRO.Value}");
//             x++;
//             moveDirection.ValueRW.Value = moveInput.ValueRO.Value;
//         }
//     }
// }