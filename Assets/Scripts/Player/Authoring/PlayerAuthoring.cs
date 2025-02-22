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
        
        AddComponent<PlayerTag>(entity);
        AddComponent<PlayerMoveInput>(entity);
        AddComponent<PlayerMousePositionInput>(entity);
        AddComponent<PlayerSpawnItemInput>(entity);
        AddComponent<PlayerPickUpItemInput>(entity);
        AddComponent<PlayerDropItemInput>(entity);
        AddComponent<PlayerMoveDirection>(entity);
        AddComponent<MoveDirection>(entity);
        AddComponent<MovingTag>(entity);
        AddComponent<CurrentSlot>(entity);
        
        AddBuffer<ItemSlot>(entity);
        for (int i = 0; i < 6; i++)
            AppendToBuffer(entity, new ItemSlot());

        SetComponentEnabled<MovingTag>(entity, false);
    }
}
