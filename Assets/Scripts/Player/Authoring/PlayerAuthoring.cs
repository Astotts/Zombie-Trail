using NUnit.Framework.Constraints;
using Unity.Entities;
using Unity.VisualScripting;
using UnityEngine;

class PlayerAuthoring : MonoBehaviour
{
}

class PlayerTagAuthoringBaker : Baker<PlayerAuthoring>
{
    public override void Bake(PlayerAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        
        AddComponent(entity, new PlayerTag());
        AddComponent(entity, new PlayerMoveInput());
        AddComponent(entity, new PlayerMousePositionInput());
        AddComponent(entity, new PlayerSpawnItemInput());
        AddComponent(entity, new PlayerPickUpItemInput());
        AddComponent(entity, new PlayerDropItemInput());
        AddComponent(entity, new PlayerMoveDirection());
        AddComponent(entity, new MoveDirection());
        AddComponent(entity, new MovingTag());
        SetComponentEnabled<MovingTag>(entity, false);
        AddBuffer<ItemSlot>(entity);
    }
}
