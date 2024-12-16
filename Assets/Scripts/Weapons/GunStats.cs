using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "WeaponStats/GunStats", fileName = "New Gun Stats")]
public class GunStats : ScriptableObject
{
    public string gunName;              // Weapon Name
    public Sprite sprite;               // Sprite to display
    public EAmmoType ammoType;          // Type of ammo to use
    public Vector2 bulletSpawnOffset;   // Start location for bullets
    public int recoil;                  // The gun would jump counter clockwise a bit (or upward if we decide to flip the gun left and right on rotate)
    public int accuracy;                // The spread of bullets (angle)
    public int range;                   // Distance from spawnOffset before disappear
    public int magazineSize;            // Number of bullets for a magazine
    public float bulletVelocity;        // Initial velocity for bullets
    public float fireRate;              // Cooldown per shot in seconds
    public float reloadTime;            // Reload time in seconds
    public float weight;                // Incase we implement a weight system
}

public enum EAmmoType
{
    PISTOL,
    RIFLE,
    SHOTGUN,
    SNIPER,
    GRENADE,
    BLANK
}