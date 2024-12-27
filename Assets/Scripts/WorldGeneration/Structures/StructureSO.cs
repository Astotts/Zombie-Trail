using UnityEngine;

[CreateAssetMenu(menuName = "Data/StructureData", fileName = "New Structure Data")]
public class StructureSO : ScriptableObject
{
    [field: SerializeField] public Sprite Sprite { get; private set; }
    [field: SerializeField] public Vector2 ColliderOffset { get; private set; }
    [field: SerializeField] public Vector2 ColliderSize { get; private set; }
}