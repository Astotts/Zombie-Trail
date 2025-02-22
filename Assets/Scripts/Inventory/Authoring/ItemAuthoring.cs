using Unity.Entities;
using UnityEngine;

class ItemAuthoring : MonoBehaviour
{
    public Vector2 centerPositionOffset;
    public Vector2 rotatePositionOffset;
    public float eulerRotationOffset;
}

class ItemAuthoringBaker : Baker<ItemAuthoring>
{
    public override void Bake(ItemAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new ItemTag());
        AddComponent(entity, new ItemOffset {
            CenterPosition = authoring.centerPositionOffset,
            RotatePosition = authoring.rotatePositionOffset,
            EulerRotation = authoring.eulerRotationOffset,
        });
    }
}
