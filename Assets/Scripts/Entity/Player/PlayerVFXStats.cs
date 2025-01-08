using UnityEngine;

[CreateAssetMenu(menuName = "Player/VFXStats", fileName = "New Player VFX Stats")]
public class PlayerVFXStats : ScriptableObject
{
    [field: SerializeField] public float OnDamagedRedFlashTime { get; private set; }
}