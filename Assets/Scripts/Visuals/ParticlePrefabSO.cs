using UnityEngine;

[CreateAssetMenu(menuName = "Stats/Particle/Prefab")]
public class ParticlePrefabSO : ScriptableObject
{
    [field: SerializeField] public GameObject Prefab { get; private set; }
}