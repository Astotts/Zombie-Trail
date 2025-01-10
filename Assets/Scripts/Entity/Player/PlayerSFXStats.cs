using UnityEngine;

[CreateAssetMenu(menuName = "Stats/Player/SFX", fileName = "New Player SFX Stats")]
public class PlayerSFXStats : ScriptableObject
{
    [field: SerializeField] public RandomPitchSound[] OnWalkedSFX;
    [field: SerializeField] public RandomPitchSound[] OnDamagedSFX;
}