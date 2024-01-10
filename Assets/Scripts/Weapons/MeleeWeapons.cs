using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapons : WeaponsClass
{
    void Update()
    {

    }

    // variables used from base(Parent): RangeOfAttack and DirectionOfAttack
    public override void Attack()
    {

        Debug.Log("MeleeWeapons Attack() function used.");
    }

    // after each attack this function is called in order to serve as a cooldown between attacks
    // variables used from base(Parent): ReloadSpeed
    public override void Reload()
    {

        Debug.Log("MeleeWeapons Reload() function used.");
    }
}
