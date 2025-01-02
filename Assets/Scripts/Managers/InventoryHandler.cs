using System.Collections;
using System.ComponentModel;
using System.Data.Common;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor.Build.Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using static EventManager;

[RequireComponent(typeof(NetworkObject))]
public class InventoryHandler : NetworkBehaviour
{
    public static readonly int INVENTORY_SIZE = 4;
    public NetworkObject Owner { get; private set; }
    public ulong OwnerID { get; private set; }

    [SerializeField] private float pickUpRadius;
    [SerializeField] private GameObject pickUpButtonGO;
    [SerializeField] private LayerMask layerToDetect;
    [SerializeField] private AvailableItemsSO availableItems;

    private readonly IItem[] inventory = new IItem[INVENTORY_SIZE];
    private PlayerControls playerControls;
    private int currentSlot = 0;
    private GameObject closestGO = null;


    void Awake()
    {
        Owner = GetComponent<NetworkObject>();
    }

    private void OnItemRightClickReleased(InputAction.CallbackContext context)
    {
        RightClickReleaseCurrentItemServerRpc();
    }

    private void OnItemRightClickPressed(InputAction.CallbackContext context)
    {
        RightClickPressedCurrentItemServerRpc();
    }

    private void OnItemLeftClickReleased(InputAction.CallbackContext context)
    {
        LeftClickReleasedCurrentItemServerRpc();
    }

    private void OnItemLeftClickPressed(InputAction.CallbackContext context)
    {
        LeftClickPressedCurrentItemServerRpc();
    }

    private void OnDropButtonPressed(InputAction.CallbackContext context)
    {
        DropCurrentItemServerRpc();
    }

    private void OnPickUpButtonPressed(InputAction.CallbackContext context)
    {
        PickUpClosestItemServerRpc();
    }

    private void OnReloadButtonPressed(InputAction.CallbackContext context)
    {
        ReloadCurrentItemServerRpc();
    }

    private void OnWeaponHotbarPressed(InputAction.CallbackContext context)
    {
        SwapItemToNewSlotServerRpc((int)context.ReadValue<float>());
    }

    // ==========================================
    // Client Side Behaviours
    // ==========================================
    public override void OnNetworkSpawn()
    {
        // We will do calculation for nearest item on Server and Client
        // But only client(owner of the object) should display the pickup button
        StartCoroutine(PickUpLoop());

        if (!IsOwner)
            return;
        playerControls = new PlayerControls();

        playerControls.Equipment.WeaponHotbar.performed += OnWeaponHotbarPressed;
        playerControls.Equipment.PickUpItem.performed += OnPickUpButtonPressed;
        playerControls.Equipment.DropItem.performed += OnDropButtonPressed;
        playerControls.Equipment.ReloadItem.performed += OnReloadButtonPressed;
        playerControls.Equipment.ItemLeftClick.started += OnItemLeftClickPressed;
        playerControls.Equipment.ItemLeftClick.canceled += OnItemLeftClickReleased;
        playerControls.Equipment.ItemRightClick.started += OnItemRightClickPressed;
        playerControls.Equipment.ItemRightClick.canceled += OnItemRightClickReleased;
        playerControls.Equipment.Enable();
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner)
            return;

        base.OnDestroy();
        playerControls.Equipment.Disable();
        playerControls.Equipment.WeaponHotbar.performed -= OnWeaponHotbarPressed;
        playerControls.Equipment.PickUpItem.performed -= OnPickUpButtonPressed;
        playerControls.Equipment.DropItem.performed -= OnDropButtonPressed;
        playerControls.Equipment.ReloadItem.performed -= OnReloadButtonPressed;
        playerControls.Equipment.ItemLeftClick.started -= OnItemLeftClickPressed;
        playerControls.Equipment.ItemLeftClick.canceled -= OnItemLeftClickReleased;
        playerControls.Equipment.ItemRightClick.started -= OnItemRightClickPressed;
        playerControls.Equipment.ItemRightClick.canceled -= OnItemRightClickReleased;
        playerControls = null;
    }

    IEnumerator PickUpLoop()
    {
        yield return new WaitForSeconds(0.5f);
        HandlePickUpDistance();
        StartCoroutine(PickUpLoop());
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

        if (!IsOwner)
            return;

        if (closestGO == null)
        {
            HidePickUpButton();
            return;
        }

        DisplayPickUpButton(closestGO.transform.position);
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

    // ==========================================
    // Server Side Behaviours
    // ==========================================

    [Rpc(SendTo.ClientsAndHost)]
    public void SetOwnerIDClientRpc(ulong ownerID)
    {
        OwnerID = ownerID;
    }

    [Rpc(SendTo.Server)]
    void RightClickPressedCurrentItemServerRpc(RpcParams rpcParams = default)
    {
        ItemRightClickPressedEventArgs eventArgs = new()
        {
            PlayerID = rpcParams.Receive.SenderClientId,
            Item = inventory[currentSlot],
            Slot = currentSlot
        };

        EventManager.EventHandler.OnItemRightClickPressed(eventArgs);
    }

    [Rpc(SendTo.Server)]
    void RightClickReleaseCurrentItemServerRpc(RpcParams rpcParams = default)
    {
        ItemRightClickReleasedEventArgs eventArgs = new()
        {
            PlayerID = rpcParams.Receive.SenderClientId,
            Item = inventory[currentSlot],
            Slot = currentSlot
        };

        EventManager.EventHandler.OnItemRightClickReleased(eventArgs);
    }

    [Rpc(SendTo.Server)]
    void LeftClickPressedCurrentItemServerRpc(RpcParams rpcParams = default)
    {
        ItemLeftClickPressedEventArgs eventArgs = new()
        {
            PlayerID = rpcParams.Receive.SenderClientId,
            Item = inventory[currentSlot],
            Slot = currentSlot
        };

        EventManager.EventHandler.OnItemLeftClickPressed(eventArgs);
    }

    [Rpc(SendTo.Server)]
    void LeftClickReleasedCurrentItemServerRpc(RpcParams rpcParams = default)
    {
        ItemLeftClickReleasedEventArgs eventArgs = new()
        {
            PlayerID = rpcParams.Receive.SenderClientId,
            Item = inventory[currentSlot],
            Slot = currentSlot
        };

        EventManager.EventHandler.OnItemLeftClickReleased(eventArgs);
    }

    [Rpc(SendTo.Server)]
    void SwapItemToNewSlotServerRpc(int newSlot, RpcParams rpcParams = default)
    {
        int previousSlot = currentSlot;
        currentSlot = newSlot;

        ItemSwappedEventArgs eventArgs = new()
        {
            PlayerID = rpcParams.Receive.SenderClientId,
            PreviousItem = inventory[previousSlot],
            PreviousSlot = previousSlot,
            CurrentItem = inventory[currentSlot],
            CurrentSlot = currentSlot
        };

        EventManager.EventHandler.OnItemSwapped(eventArgs);
    }

    [Rpc(SendTo.Server)]
    void PickUpClosestItemServerRpc(RpcParams rpcParams = default)
    {
        if (closestGO == null)
            return;

        int pickUpSlot = currentSlot;
        for (int i = 0; i < INVENTORY_SIZE; i++)
        {
            if (inventory[i] == null)
                continue;
            pickUpSlot = i;
        }

        IItem pickedUpItem = closestGO.GetComponent<IItem>();
        inventory[pickUpSlot] = pickedUpItem;

        NetworkObject itemNetworkObject = pickedUpItem.WeaponNetworkObject;
        itemNetworkObject.TrySetParent(Owner.transform);
        itemNetworkObject.ChangeOwnership(rpcParams.Receive.SenderClientId);
        PickUpItemClientRpc(itemNetworkObject, pickUpSlot == currentSlot);
        pickedUpItem.OnPickUp(this);

        ItemPickedUpEventArgs eventArgs = new()
        {
            PlayerID = rpcParams.Receive.SenderClientId,
            Item = pickedUpItem,
            CurrentSlot = currentSlot,
            PickedUpSlot = pickUpSlot
        };

        EventManager.EventHandler.OnItemPickedUp(eventArgs);
    }

    [Rpc(SendTo.ClientsAndHost)]
    void PickUpItemClientRpc(NetworkObjectReference networkObjectReference, bool isActive)
    {
        if (networkObjectReference.TryGet(out NetworkObject itemNetworkObject))
        {
            itemNetworkObject.transform.localPosition = Vector2.zero;
            itemNetworkObject.gameObject.SetActive(isActive);
        }
    }

    [Rpc(SendTo.Server)]
    void DropCurrentItemServerRpc(RpcParams rpcParams = default)
    {
        IItem droppedItem = inventory[currentSlot];
        droppedItem.OnDrop(this);
        inventory[currentSlot] = null;

        NetworkObject itemNetworkObject = droppedItem.WeaponNetworkObject;
        itemNetworkObject.TryRemoveParent();
        DropitemClientRpc(itemNetworkObject);

        ItemDroppedEventArgs eventArgs = new()
        {
            PlayerID = rpcParams.Receive.SenderClientId,
            Item = droppedItem,
            Slot = currentSlot
        };

        EventManager.EventHandler.OnItemDropped(eventArgs);
    }
    [Rpc(SendTo.ClientsAndHost)]
    void DropitemClientRpc(NetworkObjectReference networkObjectReference)
    {
        if (networkObjectReference.TryGet(out NetworkObject itemNetworkObject))
        {
            itemNetworkObject.gameObject.SetActive(true);
        }
    }

    [Rpc(SendTo.Server)]
    void ReloadCurrentItemServerRpc(RpcParams rpcParams = default)
    {
        ItemReloadEventArgs eventArgs = new()
        {
            PlayerID = rpcParams.Receive.SenderClientId,
            Item = inventory[currentSlot],
            Slot = currentSlot,
        };

        EventManager.EventHandler.OnItemReload(eventArgs);
    }

    public void LoadData(PlayerData playerData)
    {
        currentSlot = playerData.CurrentInventorySlot;
        ItemData[] itemDatas = playerData.Inventory;
        if (itemDatas == null)
            return;
        for (int i = 0; i < itemDatas.Length; i++)
        {
            ItemData data = itemDatas[i];
            if (data == null)
                continue;

            GameObject prefab = availableItems.GetItemPrefabFromID(data.Id);
            NetworkObject itemNetworkObject = NetworkObjectPool.Singleton.GetNetworkObject(prefab, Vector3.zero, Quaternion.identity);
            IItem item = itemNetworkObject.GetComponent<IItem>();
            itemNetworkObject.SpawnWithOwnership(OwnerID);
            itemNetworkObject.TrySetParent(transform, false);
            item.LoadData(data);
            item.OnPickUp(this);
            inventory[i] = item;
            PickUpItemClientRpc(itemNetworkObject, i == currentSlot);

            InventoryLoadedEventArgs eventArgs = new()
            {
                PlayerID = OwnerID,
                CurrentSlot = currentSlot,
                Item = item,
                LoadedSlot = i,
            };

            EventHandler.OnInventoryLoaded(eventArgs);
        }
    }

    public void SaveData(ref PlayerData playerData)
    {
        playerData.CurrentInventorySlot = currentSlot;
        ItemData[] itemDatas = new ItemData[4];
        for (int i = 0; i < INVENTORY_SIZE; i++)
        {
            IItem item = inventory[i];
            if (item == null)
                continue;

            ItemData data = new()
            {
                Id = item.Id
            };
            item.SaveData(ref data);
            itemDatas[i] = data;
        }
        playerData.Inventory = itemDatas;
    }
}