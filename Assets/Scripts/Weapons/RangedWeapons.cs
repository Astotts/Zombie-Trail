using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeapons : WeaponsClass
{
    [SerializeField] private float RecoilTime; // the amount of time it takes between each shot - so the recoil of the weapon
    [SerializeField] private int ClipSize; // IF WE WANT, so after you shoot a certain amount of times the Reload() function
    // will be called or we will scratch this and only use reload as a cooldown or the recoil time between attacks like it is with melee
    public int Ammo; // the total amount of ammo this zombie/player has which is set at the beginning of the wave

    void Start()
    {
        // get Ammo from wherever we're getting that
    }

    // variables used from base(Parent): RangeOfAttack and DirectionOfAttack
    public override void Attack()
    {
        
        Debug.Log("RangedWeapons Attack() function used.");
    }

    // after a certain amount of ammo is used this function is called, and if there is no ammo left - you are done
    // variables used from base(Parent): ReloadSpeed
    public override void Reload()
    {

        Debug.Log("RangedWeapons Reload() function used.");
    }
}
