using UnityEngine;

[CreateAssetMenu(menuName = "Stats/Player/VFX", fileName = "New Player VFX Stats")]
public class PlayerVFXStats : ScriptableObject
{
    [field: SerializeField] public float OnDamagedRedFlashTime { get; private set; }
}