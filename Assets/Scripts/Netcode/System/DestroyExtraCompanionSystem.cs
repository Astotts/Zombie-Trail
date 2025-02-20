using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
[UpdateAfter(typeof(GoInGameServerSystem))]
partial struct DestroyExtraCompanionSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<NeedDestroyComponentServerSide>();
    }

    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ecb = new(Allocator.Temp);
        foreach
        (
            var (spriteRenderer, entity)
            in
            SystemAPI
                .Query<RefRO<NeedDestroyComponentServerSide>>()
                .WithEntityAccess()
        )
        {
            var SpriteRenderer = state.EntityManager.GetComponentObject<SpriteRenderer>(entity);
            GameObject.Destroy(SpriteRenderer.gameObject);
            ecb.RemoveComponent<NeedDestroyComponentServerSide>(entity);
        }

        ecb.Playback(state.EntityManager);

        ecb.Dispose();
    }
}