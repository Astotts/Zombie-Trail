using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Authoring;

[UpdateAfter(typeof(RigidbodyBakingSystem))]
[WorldSystemFilter(WorldSystemFilterFlags.BakingSystem)]
partial struct InitializeInertiaSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (
            var (mass, initialInertia)
            in
            SystemAPI
                .Query<RefRW<PhysicsMass>, RefRO<InitialInertia>>()
                .WithOptions(EntityQueryOptions.IncludePrefab | EntityQueryOptions.IgnoreComponentEnabledState)
        )
        {
            mass.ValueRW.InverseInertia[0] = initialInertia.ValueRO.InfX ? 0 : mass.ValueRW.InverseInertia[0];
            mass.ValueRW.InverseInertia[1] = initialInertia.ValueRO.InfY ? 0 : mass.ValueRW.InverseInertia[1];
            mass.ValueRW.InverseInertia[2] = initialInertia.ValueRO.InfZ ? 0 : mass.ValueRW.InverseInertia[2];
        }
    }
}
