using FishNet.Object;
using UnityEngine;

public interface IWeapon
{
    public AbstractWeaponStats Stats { get; }
    public NetworkBehaviour NetworkBehaviour { get; }
    public void OnAttack(Player player);
    public void OnUse(Player player);
    public void OnDrop(Player player);
    public void OnPickUp(Player player);
    public void OnSwapIn(Player player);
    public void OnSwapOut(Player player);
}