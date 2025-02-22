using Unity.Entities;
using UnityEngine;

class ItemAuthoring : MonoBehaviour
{
    
}

class ItemAuthoringBaker : Baker<ItemAuthoring>
{
    public override void Bake(ItemAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new ItemTag());
    }
}
