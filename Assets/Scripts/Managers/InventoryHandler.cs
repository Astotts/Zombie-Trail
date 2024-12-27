using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using JetBrains.Annotations;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.VisualScripting;
using Unity.VisualScripting.ReorderableList;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.Search;
using UnityEditor.U2D.Sprites;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

[RequireComponent(typeof(NetworkObject))]
public class InventoryHandler : MonoBehaviour
{
    public static readonly int INVENTORY_SIZE = 4;
    public event EventHandler<ItemLeftClickPressedEventArgs> OnItemLeftClickPressedEvent;
    public event EventHandler<ItemLeftClickReleasedEventArgs> OnItemLeftClickReleasesedEvent;
    public event EventHandler<ItemRightClickPressedEventArgs> OnItemRightClickPressedEvent;
    public event EventHandler<ItemRightClickReleasedEventArgs> OnItemRightClickReleasedEvent;
    public event EventHandler<ItemSwappedEventArgs> OnItemSwapEvent;
    public event EventHandler<ItemPickedUpEventArgs> OnItemPickedUpEvent;
    public event EventHandler<ItemDroppedEventArgs> OnItemDroppedEvent;
    public event EventHandler<ItemReloadEventArgs> OnitemReloadEvent;
    public NetworkObject owner { get; private set; }

    [SerializeField] private float pickUpRadius;
    [SerializeField] private GameObject pickUpButtonGO;
    [SerializeField] private LayerMask layerToDetect;

    private readonly IItem[] inventory = new IItem[INVENTORY_SIZE];
    private PlayerControls playerControls;
    private int currentSlot = 0;
    private GameObject closestGO = null;

    void Awake()
    {
        owner = GetComponent<NetworkObject>();
        playerControls = new PlayerControls();

        playerControls.Equipment.WeaponHotbar.performed += OnWeaponHotbarPressed;
        playerControls.Equipment.PickUpItem.performed += OnPickUpButtonPressed;
        playerControls.Equipment.DropItem.performed += OnDropButtonPressed;
        playerControls.Equipment.ReloadItem.performed += OnReloadButtonPressed;
        playerControls.Equipment.ItemLeftClick.started += OnItemLeftClickPressed;
        playerControls.Equipment.ItemLeftClick.canceled += OnItemLeftClickReleased;
        playerControls.Equipment.ItemRightClick.started += OnItemRightClickPressed;
        playerControls.Equipment.ItemRightClick.canceled += OnItemRightClickReleased;
    }
    void OnDestroy()
    {
        Debug.Log("Inventory Handler Unsubscribing...");
        playerControls.Equipment.WeaponHotbar.performed -= OnWeaponHotbarPressed;
        playerControls.Equipment.PickUpItem.performed -= OnPickUpButtonPressed;
        playerControls.Equipment.DropItem.performed -= OnDropButtonPressed;
        playerControls.Equipment.ReloadItem.performed -= OnReloadButtonPressed;
        playerControls.Equipment.ItemLeftClick.started -= OnItemLeftClickPressed;
        playerControls.Equipment.ItemLeftClick.canceled -= OnItemLeftClickReleased;
        playerControls.Equipment.ItemRightClick.started -= OnItemRightClickPressed;
        playerControls.Equipment.ItemRightClick.canceled -= OnItemRightClickReleased;
        Debug.Log("Inventory Handler Unsubscribed Sucessfully");
    }

    void OnEnable()
    {
        playerControls.Equipment.Enable();
    }

    void OnDisable()
    {
        playerControls.Equipment.Disable();
    }

    private void OnItemRightClickReleased(InputAction.CallbackContext context)
    {
        RightClickReleaseCurrentItem();
    }

    private void OnItemRightClickPressed(InputAction.CallbackContext context)
    {
        RightClickPressedCurrentItem();
    }

    private void OnItemLeftClickReleased(InputAction.CallbackContext context)
    {
        LeftClickReleasedCurrentItem();
    }

    private void OnItemLeftClickPressed(InputAction.CallbackContext context)
    {
        LeftClickPressedCurrentItem();
    }

    private void OnDropButtonPressed(InputAction.CallbackContext context)
    {
        DropCurrentItem();
    }

    private void OnPickUpButtonPressed(InputAction.CallbackContext context)
    {
        PickUpClosestItem();
    }

    private void OnReloadButtonPressed(InputAction.CallbackContext context)
    {
        ReloadCurrentItem();
    }

    private void OnWeaponHotbarPressed(InputAction.CallbackContext context)
    {
        SwapItemToNewSlot((int)context.ReadValue<float>());
    }

    void Update()
    {
        HandlePickUpDistance();
    }

    void HandlePickUpDistance()
    {
        Collider2D[] itemsAroundPlayer = Physics2D.OverlapCircleAll(transform.position, pickUpRadius, layerToDetect);
        float closestDistance = float.MaxValue;
        closestGO = null;
        foreach (Collider2D collider2D in itemsAroundPlayer)
        {
            float currentDistance = Vector2.Distance(collider2D.transform.position, this.transform.position);
            if (closestDistance > currentDistance)
            {
                closestDistance = currentDistance;
                closestGO = collider2D.gameObject;
            }
        }
        if (closestGO == null)
        {
            HidePickUpButton();
            return;
        }

        DisplayPickUpButton(closestGO.transform.position);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, pickUpRadius);
    }

    void HidePickUpButton()
    {
        pickUpButtonGO.SetActive(false);
    }
    void DisplayPickUpButton(Vector2 itemPosition)
    {
        float xPos = transform.position.x + (itemPosition.x - transform.position.x) / 5;
        float yPos = transform.position.y + (itemPosition.y - transform.position.y) / 5;
        Vector2 position = new(xPos, yPos);

        pickUpButtonGO.transform.position = position;
        pickUpButtonGO.SetActive(true);
    }
    void RightClickPressedCurrentItem()
    {
        ItemRightClickPressedEventArgs eventArgs = new()
        {
            Item = inventory[currentSlot],
            Slot = currentSlot
        };

        OnItemRightClickPressed(eventArgs);
    }

    void RightClickReleaseCurrentItem()
    {
        ItemRightClickReleasedEventArgs eventArgs = new()
        {
            Item = inventory[currentSlot],
            Slot = currentSlot
        };

        OnItemRightClickReleased(eventArgs);
    }

    void LeftClickPressedCurrentItem()
    {
        ItemLeftClickPressedEventArgs eventArgs = new()
        {
            Item = inventory[currentSlot],
            Slot = currentSlot
        };

        OnItemLeftClickPressed(eventArgs);
    }


    void LeftClickReleasedCurrentItem()
    {
        ItemLeftClickReleasedEventArgs eventArgs = new()
        {
            Item = inventory[currentSlot],
            Slot = currentSlot
        };

        OnItemLeftClickReleased(eventArgs);
    }

    void SwapItemToNewSlot(int newSlot)
    {
        int previousSlot = currentSlot;
        currentSlot = newSlot;

        ItemSwappedEventArgs eventArgs = new()
        {
            PreviousItem = inventory[previousSlot],
            PreviousSlot = previousSlot,
            CurrentItem = inventory[currentSlot],
            CurrentSlot = currentSlot
        };

        OnItemSwapped(eventArgs);
    }

    void PickUpClosestItem()
    {
        if (closestGO == null)
            return;

        if (inventory[currentSlot] != null)
            DropCurrentItem();
        IItem pickedUpItem = closestGO.GetComponent<IItem>();
        inventory[currentSlot] = pickedUpItem;
        pickedUpItem.OnPickUp(this);

        PickUpItemServerRpc(pickedUpItem.WeaponNetworkObject);

        ItemPickedUpEventArgs eventArgs = new()
        {
            Item = pickedUpItem,
            Slot = currentSlot
        };

        OnItemPickedUp(eventArgs);
    }

    void DropCurrentItem()
    {
        IItem droppedItem = inventory[currentSlot];
        droppedItem.OnDrop(this);
        inventory[currentSlot] = null;

        DropItemServerRpc(droppedItem.WeaponNetworkObject);

        ItemDroppedEventArgs eventArgs = new()
        {
            Item = droppedItem,
            Slot = currentSlot
        };

        OnItemDropped(eventArgs);
    }

    void ReloadCurrentItem()
    {
        ItemReloadEventArgs eventArgs = new()
        {
            Item = inventory[currentSlot],
            Slot = currentSlot,
        };

        OnItemReload(eventArgs);
    }

    [Rpc(SendTo.Server)]
    void DropItemServerRpc(NetworkObject networkObject)
    {
        networkObject.TrySetParent((Transform)null, true);
        networkObject.gameObject.layer = LayerMask.NameToLayer("PickUpRaycast");
        networkObject.gameObject.SetActive(true);
    }

    [Rpc(SendTo.Server)]
    void PickUpItemServerRpc(NetworkObject newItem)
    {
        newItem.TrySetParent(owner.transform, false);
        newItem.gameObject.layer = LayerMask.NameToLayer("IgnorePickUpRaycast");
        newItem.transform.localPosition = Vector2.zero;
    }

    #region ItemLeftClickPressedEvent
    void OnItemLeftClickPressed(ItemLeftClickPressedEventArgs eventArgs)
    {
        OnItemLeftClickPressedEvent?.Invoke(this, eventArgs);
    }
    public class ItemLeftClickPressedEventArgs : EventArgs
    {
        public IItem Item { get; set; }
        public int Slot { get; set; }
    }
    #endregion

    #region ItemLeftClickReleasedEvent
    void OnItemLeftClickReleased(ItemLeftClickReleasedEventArgs eventArgs)
    {
        OnItemLeftClickReleasesedEvent?.Invoke(this, eventArgs);
    }
    public class ItemLeftClickReleasedEventArgs : EventArgs
    {
        public IItem Item { get; set; }
        public int Slot { get; set; }
    }
    #endregion

    #region ItemRightClickPressedEvent
    void OnItemRightClickPressed(ItemRightClickPressedEventArgs eventArgs)
    {
        OnItemRightClickPressedEvent?.Invoke(this, eventArgs);
    }
    public class ItemRightClickPressedEventArgs : EventArgs
    {
        public IItem Item { get; set; }
        public int Slot { get; set; }
    }
    #endregion

    #region ItemRightClickReleasedEvent
    void OnItemRightClickReleased(ItemRightClickReleasedEventArgs eventArgs)
    {
        OnItemRightClickReleasedEvent?.Invoke(this, eventArgs);
    }
    public class ItemRightClickReleasedEventArgs : EventArgs
    {
        public IItem Item { get; set; }
        public int Slot { get; set; }
    }
    #endregion

    #region ItemSwapEvent
    void OnItemSwapped(ItemSwappedEventArgs eventArgs)
    {
        OnItemSwapEvent?.Invoke(this, eventArgs);
    }

    public class ItemSwappedEventArgs : EventArgs
    {
        public IItem PreviousItem { get; set; }
        public int PreviousSlot { get; set; }
        public IItem CurrentItem { get; set; }
        public int CurrentSlot { get; set; }
    }
    #endregion

    #region ItemPickedUpEvent
    void OnItemPickedUp(ItemPickedUpEventArgs eventArgs)
    {
        OnItemPickedUpEvent?.Invoke(this, eventArgs);
    }

    public class ItemPickedUpEventArgs : EventArgs
    {
        public IItem Item { get; set; }
        public int Slot { get; set; }
    }
    #endregion

    #region ItemDroppedEvent
    void OnItemDropped(ItemDroppedEventArgs eventArgs)
    {
        OnItemDroppedEvent?.Invoke(this, eventArgs);
    }

    public class ItemDroppedEventArgs : EventArgs
    {
        public IItem Item { get; set; }
        public int Slot { get; set; }
    }
    #endregion

    #region ItemReloadEvent

    void OnItemReload(ItemReloadEventArgs eventArgs)
    {
        OnitemReloadEvent?.Invoke(this, eventArgs);
    }

    public class ItemReloadEventArgs : EventArgs
    {
        public IItem Item { get; set; }
        public int Slot { get; set; }
    }
    #endregion
}

public interface IItem
{
    NetworkObject WeaponNetworkObject { get; }
    void OnPickUp(InventoryHandler playerInventory);
    void OnDrop(InventoryHandler playerInventory);
}

public interface IReloadableItem
{
    event EventHandler<float> OnReloadEvent;
}