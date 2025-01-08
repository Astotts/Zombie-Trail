using UnityEngine;

[CreateAssetMenu(menuName = "Player/SFXStats", fileName = "New Player SFX Stats")]
public class PlayerSFXStats : ScriptableObject
{
    [field: SerializeField] public RandomPitchSound[] OnWalkedSFX;
    [field: SerializeField] public RandomPitchSound[] OnDamagedSFX;
}