using System;
using System.ComponentModel;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

public class EventManager
{
    private static readonly Lazy<EventManager> lazy = new(() => new EventManager());
    public static EventManager EventHandler { get { return lazy.Value; } }
    #region Events
    public event EventHandler<InventoryLoadedEventArgs> OnInventoryLoadedEvent;
    public event EventHandler<ItemLeftClickPressedEventArgs> OnItemLeftClickPressedEvent;
    public event EventHandler<ItemLeftClickReleasedEventArgs> OnItemLeftClickReleasesedEvent;
    public event EventHandler<ItemRightClickPressedEventArgs> OnItemRightClickPressedEvent;
    public event EventHandler<ItemRightClickReleasedEventArgs> OnItemRightClickReleasedEvent;
    public event EventHandler<ItemSwapPressedEventArgs> OnItemSwapPressedEvent;
    public event EventHandler<ItemPickUpPressedEventArgs> OnItemPickUpPressedEvent;
    public event EventHandler<ItemDropPressedEventArgs> OnItemDropPressedEvent;
    public event EventHandler<ItemReloadPressedEventArgs> OnItemReloadPressedEvent;
    public event EventHandler<AmmoChangedEventArgs> OnAmmoChangedEvent;
    public event EventHandler<GunReloadEventArgs> OnGunReloadEvent;
    public event EventHandler<GunReloadInterruptedEventArgs> OnGunReloadInterruptedEvent;
    public event EventHandler<GunReloadedEventArgs> OnGunReloadedEvent;
    public event EventHandler<PlayerDamagedEventArgs> OnPlayerDamagedEvent;
    public event EventHandler<PlayerHealthLoadedEventArgs> OnPlayerHealthLoadedEvent;

    #endregion

    #region InventoryLoadedEvent
    public void OnInventoryLoaded(InventoryLoadedEventArgs eventArgs)
    {
        OnInventoryLoadedEvent?.Invoke(this, eventArgs);
    }
    public class InventoryLoadedEventArgs : EventArgs
    {
        public ulong PlayerID { get; set; }
        public int CurrentSlot { get; set; }
        public IItem Item { get; set; }
        public int LoadedSlot { get; set; }
    }
    #endregion

    #region ItemLeftClickPressedEvent
    public void OnItemLeftClickPressed(ItemLeftClickPressedEventArgs eventArgs)
    {
        OnItemLeftClickPressedEvent?.Invoke(this, eventArgs);
    }
    public class ItemLeftClickPressedEventArgs : EventArgs
    {
        public ulong PlayerID { get; set; }
        public IItem Item { get; set; }
        public int Slot { get; set; }
    }
    #endregion

    #region ItemLeftClickReleasedEvent
    public void OnItemLeftClickReleased(ItemLeftClickReleasedEventArgs eventArgs)
    {
        OnItemLeftClickReleasesedEvent?.Invoke(this, eventArgs);
    }
    public class ItemLeftClickReleasedEventArgs : EventArgs
    {
        public ulong PlayerID { get; set; }
        public IItem Item { get; set; }
        public int Slot { get; set; }
    }
    #endregion

    #region ItemRightClickPressedEvent
    public void OnItemRightClickPressed(ItemRightClickPressedEventArgs eventArgs)
    {
        OnItemRightClickPressedEvent?.Invoke(this, eventArgs);
    }
    public class ItemRightClickPressedEventArgs : EventArgs
    {
        public ulong PlayerID { get; set; }
        public IItem Item { get; set; }
        public int Slot { get; set; }
    }
    #endregion

    #region ItemRightClickReleasedEvent
    public void OnItemRightClickReleased(ItemRightClickReleasedEventArgs eventArgs)
    {
        OnItemRightClickReleasedEvent?.Invoke(this, eventArgs);
    }
    public class ItemRightClickReleasedEventArgs : EventArgs
    {
        public ulong PlayerID { get; set; }
        public IItem Item { get; set; }
        public int Slot { get; set; }
    }
    #endregion

    #region ItemSwapEvent
    public void OnItemSwapped(ItemSwapPressedEventArgs eventArgs)
    {
        OnItemSwapPressedEvent?.Invoke(this, eventArgs);
    }

    public class ItemSwapPressedEventArgs : EventArgs
    {
        public ulong PlayerID { get; set; }
        public IItem PreviousItem { get; set; }
        public int PreviousSlot { get; set; }
        public IItem CurrentItem { get; set; }
        public int CurrentSlot { get; set; }
    }
    #endregion

    #region ItemPickedUpEvent
    public void OnItemPickedUp(ItemPickUpPressedEventArgs eventArgs)
    {
        OnItemPickUpPressedEvent?.Invoke(this, eventArgs);
    }

    public class ItemPickUpPressedEventArgs : EventArgs
    {
        public ulong PlayerID { get; set; }
        public IItem Item { get; set; }
        public int CurrentSlot { get; set; }
        public int PickedUpSlot { get; set; }
    }
    #endregion

    #region ItemDroppedEvent
    public void OnItemDropped(ItemDropPressedEventArgs eventArgs)
    {
        OnItemDropPressedEvent?.Invoke(this, eventArgs);
    }

    public class ItemDropPressedEventArgs : EventArgs
    {
        public ulong PlayerID { get; set; }
        public IItem Item { get; set; }
        public int Slot { get; set; }
    }
    #endregion

    #region ItemReloadEvent

    public void OnItemReload(ItemReloadPressedEventArgs eventArgs)
    {
        OnItemReloadPressedEvent?.Invoke(this, eventArgs);
    }

    public class ItemReloadPressedEventArgs : EventArgs
    {
        public ulong PlayerID { get; set; }
        public IItem Item { get; set; }
        public int Slot { get; set; }
    }
    #endregion

    #region GunAmmoChangedEvent
    public void OnAmmoChanged(AmmoChangedEventArgs eventArgs)
    {
        OnAmmoChangedEvent?.Invoke(this, eventArgs);
    }
    public class AmmoChangedEventArgs : EventArgs
    {
        public ulong OwnerID { get; set; }
        public IItem Item { get; set; }
        public int PreviousValue { get; set; }
        public int CurrentValue { get; set; }
    }
    #endregion

    #region GunReloadEvent
    public void OnGunReload(GunReloadEventArgs eventArgs)
    {
        OnGunReloadEvent?.Invoke(this, eventArgs);
    }
    public class GunReloadEventArgs : EventArgs
    {
        public ulong PlayerID { get; set; }
        public IItem Item { get; set; }
        public float ReloadTime { get; set; }
    }
    #endregion

    #region GunReloadInteruptedEvent
    public void GunReloadInterupted(GunReloadInterruptedEventArgs eventArgs)
    {
        OnGunReloadInterruptedEvent?.Invoke(this, eventArgs);
    }
    public class GunReloadInterruptedEventArgs : EventArgs
    {
        public ulong PlayerID { get; set; }
        public IItem Item { get; set; }
    }
    #endregion

    #region GunReloadedEvent
    public void OnGunReloaded(GunReloadedEventArgs eventArgs)
    {
        OnGunReloadedEvent?.Invoke(this, eventArgs);
    }
    public class GunReloadedEventArgs : EventArgs
    {
        public ulong PlayerID { get; set; }
        public IItem Item { get; set; }
        public int CurrentMagazine { get; set; }
        public int MaxMagazine { get; set; }
    }
    #endregion

    #region PlayerHealthLoadedEvent

    public void OnPlayerHealthLoaded(PlayerHealthLoadedEventArgs eventArgs)
    {
        OnPlayerHealthLoadedEvent?.Invoke(this, eventArgs);
    }
    public class PlayerHealthLoadedEventArgs : EventArgs
    {
        public ulong PlayerID { get; set; }
        public float CurrentHealth { get; set; }
        public float MaxHealth { get; set; }
    }

    #endregion

    #region PlayerDamagedEvent
    public void OnPlayerDamaged(PlayerDamagedEventArgs eventArgs)
    {
        OnPlayerDamagedEvent?.Invoke(this, eventArgs);
    }
    public class PlayerDamagedEventArgs : EventArgs
    {
        public ulong PlayerID { get; set; }
        public float PreviousHealth { get; set; }
        public float CurrentHealth { get; set; }
        public float MaxHealth { get; set; }
    }
    #endregion
}