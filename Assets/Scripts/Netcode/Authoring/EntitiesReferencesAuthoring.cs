using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public class EntitiesReferencesAuthoring : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject itemPrefab;

    public class Baker : Baker<EntitiesReferencesAuthoring>
    {
        public override void Bake(EntitiesReferencesAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new EntitiesReferences()
            {
                PlayerPrefabEntity = GetEntity(authoring.playerPrefab, TransformUsageFlags.Dynamic),
                Item = GetEntity(authoring.itemPrefab, TransformUsageFlags.Dynamic),
            });
        }
    }
}

public struct EntitiesReferences : IComponentData
{
    public Entity PlayerPrefabEntity;
    public Entity Item;
}