using UnityEngine;

[CreateAssetMenu(fileName = "MeleeWeapon", menuName = "Scriptable Objects/Weapon/Melee")]
public class MeleeWeaponSO : AbstractWeaponStats
{
    [field: SerializeField] public float DamageFromStrength { get; private set; }
    [field: SerializeField] public float KnockbackFromStrength { get; private set; }
    [field: SerializeField] public float Cooldown { get; private set; }
    [field: SerializeField] public int AttackSpeed { get; private set; }
    [field: SerializeField] public Vector2 HitboxOffset { get; private set; }
    [field: SerializeField] public Vector2 HitboxSize { get; private set; }
}