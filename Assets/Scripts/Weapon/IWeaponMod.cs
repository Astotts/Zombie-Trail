using UnityEngine;

public interface IWeaponMod
{
    public void OnAttach(Player player, IWeapon weapon);
    public void OnDetach(Player player, IWeapon weapon);
    public void OnAttack(Player player, IWeapon weapon);
    public void OnUse(Player player, IWeapon weapon);
    public void OnSwapIn(Player player, IWeapon weapon);
    public void OnSwapOut(Player player, IWeapon weapon);
}