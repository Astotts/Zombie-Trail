using System;
using UnityEngine;

public class WeaponAttackEventArgs : EventArgs
{
    public Player Player { get; }
    public IWeapon Weapon { get; }

    public WeaponAttackEventArgs(Player player, IWeapon weapon) {
        Player = player;
        Weapon = weapon;
    }
}
