using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponsClass : MonoBehaviour
{
    [SerializeField] float RangeOfAttack;
    [SerializeField] int ReloadSpeed; // how long the reload / cooldown of attacks will take 
    private Vector2 DirectionOfAttack;
    public float WeightOfWeapon; // decreases speed

    // Start is called before the first frame update
    void Start()
    {
        // Get DirectionOfAttack from the player/zombie

        // WeightOfWeapon will be set in Unity for each weapon then used by the zombie/player to decrease their speed
        //NOT used in this script, rather just set with the weapon. 
    }

    // Melee Weapons: Acts as a cooldown between each attack
    // Ranged Weapons: Reloads the gun/ranged weapon
    // Variables used: ReloadSpeed
    public virtual void Reload()
    {
        // reload speed(cooldown time) will serve as the time it takes before you can attack again for both ranged and melee weapons
        // Ranged Weapons: Once a certain amount of ammo is used a coroutine(or some kind of delay implementation) is called to reload, if no more ammo - you are done. 
        // Melee Weapons: After each attack this function is called

        Debug.Log("Parent class Reload() function used.");
    }

    // variables used: RangeOfAttack and DirectionOfAttack
    public virtual void Attack()
    {
        Debug.Log("Parent class Attack() function used.");
    }
}
