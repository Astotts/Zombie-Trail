using UnityEngine;

[CreateAssetMenu(menuName = "WeaponStats/BulletStats", fileName = "New Bullet Stats")]
public class AmmoStats : ScriptableObject
{
    public string bulletName;   // Name to display on gui
    public Sprite guiSprite;    // Sprite before shots fired
    public Sprite firedSprite;  // Sprite after shots fired
    public EAmmoType ammoType;  // Ammo type for guns to use
    public int damage;          // Damage on trigger enter
    public int penetration;     // Number of zombie to hit before stopped
}