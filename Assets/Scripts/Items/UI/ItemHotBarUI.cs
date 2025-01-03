using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using static EventManager;

public class ItemHotBarUI : NetworkBehaviour
{
    [SerializeField] Image[] itemImages;

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
            return;

        EventHandler.OnItemPickUpPressedEvent += OnItemPickedUp;
        EventHandler.OnItemDropPressedEvent += OnItemDropped;
        EventHandler.OnInventoryLoadedEvent += OnInventorySlotLoaded;

        base.OnNetworkSpawn();
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer)
            return;

        EventHandler.OnItemPickUpPressedEvent -= OnItemPickedUp;
        EventHandler.OnItemDropPressedEvent -= OnItemDropped;
        EventHandler.OnInventoryLoadedEvent -= OnInventorySlotLoaded;

        base.OnNetworkDespawn();
    }

    private void OnInventorySlotLoaded(object sender, InventoryLoadedEventArgs e)
    {
        DisplayItemClientRpc(e.Item.WeaponNetworkObject, e.LoadedSlot, RpcTarget.Single(e.PlayerID, RpcTargetUse.Temp));
    }

    [Rpc(SendTo.SpecifiedInParams)]
    void DisplayItemClientRpc(NetworkObjectReference networkObjectReference, int slot, RpcParams rpcParams)
    {
        if (!networkObjectReference.TryGet(out NetworkObject networkObject))
            return;

        if (networkObject.TryGetComponent(out IDisplayableWeapon weapon))
        {
            Image imageToUpdate = itemImages[slot];
            imageToUpdate.sprite = weapon.Icon;
            imageToUpdate.SetNativeSize();
            imageToUpdate.color = Color.white;
        }
    }

    private void OnItemPickedUp(object sender, ItemPickUpPressedEventArgs e)
    {
        DisplayItemClientRpc(e.Item.WeaponNetworkObject, e.PickedUpSlot, RpcTarget.Single(e.PlayerID, RpcTargetUse.Temp));
    }

    private void OnItemDropped(object sender, ItemDropPressedEventArgs e)
    {
        RemoveItemClientRpc(e.Slot, RpcTarget.Single(e.PlayerID, RpcTargetUse.Temp));
    }

    [Rpc(SendTo.SpecifiedInParams)]
    void RemoveItemClientRpc(int slot, RpcParams rpcParams)
    {
        Image imageToUpdate = itemImages[slot];
        imageToUpdate.sprite = null;
        imageToUpdate.color = Color.clear;
    }
}