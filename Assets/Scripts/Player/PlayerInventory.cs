using System;
using System.Collections;
using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : NetworkBehaviour
{
    [SerializeField] PlayerInventoriesSO inventoriesSO;

    InputAction attackAction;
    InputAction useAction;

    void Start()
    {
        attackAction = InputSystem.actions.FindAction("Attack");
        useAction = InputSystem.actions.FindAction("Use");
    }

    public override void OnStartNetwork()
    {
        base.OnStartNetwork();

        Debug.Log($"Detecting new player {OwnerId}, initializing new inventory");
        inventoriesSO.Inventories.Add(OwnerId, new IWeapon[4]);
    }
}