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
    public event EventHandler<ItemSwappedEventArgs> OnItemSwappedEvent;
    public event EventHandler<ItemPickedUpEventArgs> OnItemPickedUpEvent;
    public event EventHandler<ItemDroppedEventArgs> OnItemDroppedEvent;
    public event EventHandler<ItemReloadEventArgs> OnitemReloadEvent;
    public event EventHandler<AmmoChangedEventArgs> OnAmmoChangedEvent;
    public event EventHandler<GunReloadEventArgs> OnGunReloadEvent;
    #endregion

    #region InventoryLoadedEvent
    public void OnInventoryLoad(InventoryLoadedEventArgs eventArgs)
    {
        OnInventoryLoadedEvent?.Invoke(this, eventArgs);
    }
    public class InventoryLoadedEventArgs : EventArgs
    {
        public ulong PlayerID { get; set; }
        public IItem Item { get; set; }
        public int Slot { get; set; }
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
    public void OnItemSwapped(ItemSwappedEventArgs eventArgs)
    {
        OnItemSwappedEvent?.Invoke(this, eventArgs);
    }

    public class ItemSwappedEventArgs : EventArgs
    {
        public ulong PlayerID { get; set; }
        public IItem PreviousItem { get; set; }
        public int PreviousSlot { get; set; }
        public IItem CurrentItem { get; set; }
        public int CurrentSlot { get; set; }
    }
    #endregion

    #region ItemPickedUpEvent
    public void OnItemPickedUp(ItemPickedUpEventArgs eventArgs)
    {
        OnItemPickedUpEvent?.Invoke(this, eventArgs);
    }

    public class ItemPickedUpEventArgs : EventArgs
    {
        public ulong PlayerID { get; set; }
        public IItem Item { get; set; }
        public int Slot { get; set; }
    }
    #endregion

    #region ItemDroppedEvent
    public void OnItemDropped(ItemDroppedEventArgs eventArgs)
    {
        OnItemDroppedEvent?.Invoke(this, eventArgs);
    }

    public class ItemDroppedEventArgs : EventArgs
    {
        public ulong PlayerID { get; set; }
        public IItem Item { get; set; }
        public int Slot { get; set; }
    }
    #endregion

    #region ItemReloadEvent

    public void OnItemReload(ItemReloadEventArgs eventArgs)
    {
        OnitemReloadEvent?.Invoke(this, eventArgs);
    }

    public class ItemReloadEventArgs : EventArgs
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

    #region GunReloadedEvent
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
}