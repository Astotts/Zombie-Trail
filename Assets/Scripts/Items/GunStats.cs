using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/GunStats", fileName = "New Gun Stats")]
public class GunStats : ScriptableObject
{
    // [field: SerializeField] Can display variables on inspector, PEAK!
    // The property (private set) is to make sure other codes don't change variables
    [field: SerializeField] public Sprite Icon { get; private set; }                // Weapon Icon on ui (Pivot at the end of the gun)
    [field: SerializeField] public Sprite WeaponSprite { get; private set; }        // Sprite to display
    [field: SerializeField] public Sprite AmmoIcon { get; private set; }            // Ammo Icon for UI
    [field: SerializeField] public Sprite EmptyAmmoIcon { get; private set; }       // Empty Ammo Icon for UI
    [field: SerializeField] public GameObject BulletGO { get; private set; }        // Ammo GameObject to spawn on shots
    [field: SerializeField] public Texture2D Cursor { get; private set; }           // Sprite for cursor
    [field: SerializeField] public float BulletSpawnOffset { get; private set; }    // Start location for bullets
    [field: SerializeField] public string GunName { get; private set; }             // Weapon Name
    [field: SerializeField] public int Damage { get; private set; }                 // Weapon Damage
    [field: SerializeField] public int Recoil { get; private set; }                 // The gun would jump counter clockwise a bit (or upward if we decide to flip the gun left and right on rotate)
    [field: SerializeField] public int Accuracy { get; private set; }               // The spread of bullets
    [field: SerializeField] public int Range { get; private set; }                  // Distance from spawnOffset before disappear
    [field: SerializeField] public int MagazineSize { get; private set; }           // Number of bullets for a magazine
    [field: SerializeField] public int Capacity { get; private set; }               // Max number of magazine player can hold
    [field: SerializeField] public float BulletVelocity { get; private set; }       // Initial velocity for bullets
    [field: SerializeField] public float FireRate { get; private set; }             // Cooldown per shot in seconds
    [field: SerializeField] public float ReloadTime { get; private set; }           // Reload time in seconds
    [field: SerializeField] public float Penetration { get; private set; }          // Zombies to hit before bullet disappear
    [field: SerializeField] public float Weight { get; private set; }               // In case we implement a weight system
    [field: SerializeField] public string[] GunShotSFXs { get; private set; }       // Gun Shots Sound Effects
    [field: SerializeField] public string[] ReloadSFXs { get; private set; }        // Reload Sound Effects
    [field: SerializeField] public string[] EquipSFXs { get; private set; }         // Equip Sound Effect
    [field: SerializeField] public string[] HolsterSFXs { get; private set; }       // Holster Sound Effect
}

public enum EAmmoType
{
    RIFLE,
    PISTOL,
    SNIPER,
    SHOTGUN,
    GRENADE
}
