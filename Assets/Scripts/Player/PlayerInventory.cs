using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : NetworkBehaviour
{
    [SerializeField] PlayerInventoriesSO inventoriesSO;

    InputAction hotbarAction;

    void OnEnable()
    {
        hotbarAction = InputSystem.actions.FindAction("HotbarSelect");
        hotbarAction.performed += OnHotbarSelect;
    }

    void OnDisable() {
        hotbarAction.performed -= OnHotbarSelect;
    }

    private void OnHotbarSelect(InputAction.CallbackContext context)
    {
        Debug.Log(context.ReadValue<float>());
    }

    public override void OnStartNetwork()
    {
        base.OnStartNetwork();

        Debug.Log($"Detecting new player {OwnerId}, initializing new inventory");
        inventoriesSO.Inventories.Add(OwnerId, new IWeapon[4]);
    }
}