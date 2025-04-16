using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : NetworkBehaviour
{
    [SerializeField] PlayerInventoriesSO inventoriesSO;

    InputAction hotbarAction;

    public override void OnStartNetwork()
    {
        base.OnStartNetwork();

        if (IsServerInitialized || Owner.IsLocalClient)
        {
            Debug.Log($"Detecting new player {OwnerId}, initializing new inventory");
            inventoriesSO.Inventories.Add(OwnerId, new IWeapon[4]);
            inventoriesSO.CurrentSlot.Add(OwnerId, 0);
        }
    }

    void Start()
    {
        hotbarAction = InputSystem.actions.FindAction("HotbarSelect");
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (!IsOwner)
            return;
        
        hotbarAction.performed += OnHotbarSelect;
    }

    void OnDisable() {
        hotbarAction.performed -= OnHotbarSelect;
    }

    private void OnHotbarSelect(InputAction.CallbackContext context)
    {
        int slotToSwap = (int)context.ReadValue<float>();
        int currentSlot = inventoriesSO.CurrentSlot[OwnerId];
        if (currentSlot == slotToSwap)
            return;
        
        SwapWeapon(currentSlot, slotToSwap);
    }

    void SwapWeapon(int prev, int next) {
        Debug.Log($"Swapping from {prev} to {next}");
        IWeapon[] ownerInventory = inventoriesSO.Inventories[OwnerId];

        IWeapon nextWeapon = ownerInventory[next];
        if (nextWeapon == null)
            return;

        // Set the prev weapon inactive if it exist
        IWeapon prevWeapon = ownerInventory[prev];
        if (prevWeapon != null)
        {
            NetworkBehaviour prevWeaponBehaviour = prevWeapon.NetworkBehaviour;
            prevWeaponBehaviour.NetworkObject.gameObject.SetActive(false);
        }
        
        NetworkBehaviour nextWeaponBehaviour = nextWeapon.NetworkBehaviour;
        nextWeaponBehaviour.NetworkObject.gameObject.SetActive(true);
        inventoriesSO.CurrentSlot[OwnerId] = next;
    }
}