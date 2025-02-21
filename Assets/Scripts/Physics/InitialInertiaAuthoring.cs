using Unity.Entities;
using UnityEngine;

class InitialInertiaAuthoring : MonoBehaviour
{
    public bool IsInfinityX;
    public bool IsInfinityY;
    public bool IsInfinityZ;
}

class InitialInertiaAuthoringBaker : Baker<InitialInertiaAuthoring>
{
    public override void Bake(InitialInertiaAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);

        AddComponent(entity, new InitialInertia {
            InfX = authoring.IsInfinityX,
            InfY = authoring.IsInfinityY,
            InfZ = authoring.IsInfinityZ
        });
    }
}
