using System;
using System.Collections;
using FishNet.Connection;
using FishNet.Object;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPickUp : NetworkBehaviour
{
    [SerializeField] PlayerInventoriesSO inventoriesSO;
    [SerializeField] float pickUpRange;
    [SerializeField] Vector2 itemOffset;
    [SerializeField] float pickUpInterval;
    [SerializeField] GameObject pickUpButton;
    [SerializeField] LayerMask weaponLayer;
    
    InputAction pickUpAction;

    Coroutine pickUpCoroutine;

    void Start()
    {
        pickUpAction = InputSystem.actions.FindAction("PickUp");

        pickUpAction.performed += OnPickUpPerformed;
    }

    private void OnPickUpPerformed(InputAction.CallbackContext context)
    {
        if (!pickUpButton.activeSelf)
            return;
        
        RequestPickup();
    }

    public override void OnStartNetwork()
    {
        base.OnStartNetwork();
        if (!Owner.IsLocalClient)
        {
            enabled = false;
            return;
        }
        
        pickUpCoroutine = StartCoroutine(PickUpLoop());
    }

    public override void OnStopNetwork()
    {
        base.OnStopNetwork();

        StopCoroutine(pickUpCoroutine);
    }

    IEnumerator PickUpLoop()
    {
        while (!inventoriesSO.Inventories.ContainsKey(OwnerId))
        {
            Debug.Log("Waiting for owner inventory to be initialized");
            yield return null;
        }
        IWeapon nearestWeapon = FindNearestWeaponInRange();
        if (nearestWeapon != null)
        {
            if (inventoriesSO.Inventories[OwnerId][(int)nearestWeapon.Stats.Type] != null)
            {
                ShowPickUpButton(nearestWeapon);
            } else {
                RequestPickup();
            }
        } else {
            HidePickUpButton();
        }

        yield return new WaitForSeconds(pickUpInterval);
        pickUpCoroutine = StartCoroutine(PickUpLoop());
    }

    IWeapon FindNearestWeaponInRange() {
        float distance = float.MaxValue;
        IWeapon abstractWeapon = null;
        Vector2 pickUpCenter = (Vector2) transform.position + itemOffset;
        foreach (Collider2D collider in Physics2D.OverlapCircleAll(pickUpCenter, pickUpRange, weaponLayer))
        {
            float currentDistance = Vector2.Distance(pickUpCenter, collider.transform.position);
            if (distance <= currentDistance)
                continue;
            distance = currentDistance;
            abstractWeapon = collider.GetComponent<IWeapon>();
        }

        return abstractWeapon;
    }

    void ShowPickUpButton(IWeapon weapon) {
        Vector3 weaponPos = weapon.NetworkBehaviour.transform.position;
        Vector3 direction = weaponPos - transform.position;

        pickUpButton.transform.position = transform.position + direction.normalized;
        pickUpButton.SetActive(true);
    }

    void HidePickUpButton() {
        pickUpButton.SetActive(false);
    }

    [ObserversRpc]
    void DropWeapon(NetworkBehaviour weaponNetworkBehaviour, Vector2 direction)
    {
        // TODO Calculate direction to throw weapon
        // Might need to add predicted rigid to weapon
    }


    [ServerRpc]
    void RequestPickup()
    {
        Debug.Log("Server handling pickup");
        IWeapon nearestWeapon = FindNearestWeaponInRange();

        if (nearestWeapon == null)
            return;

        Debug.Log("Found nearest weapon");
        int equipmentSlot = (int)nearestWeapon.Stats.Type;
        IWeapon[] ownerInventory = inventoriesSO.Inventories[OwnerId];

        IWeapon currentWeapon = ownerInventory[equipmentSlot];

        if (currentWeapon == null)
        {
            ownerInventory[equipmentSlot] = nearestWeapon;
        }
        else
        {
            NetworkBehaviour weaponNetworkB = currentWeapon.NetworkBehaviour;
            weaponNetworkB.transform.localPosition = itemOffset;
            weaponNetworkB.NetworkObject.UnsetParent();
            weaponNetworkB.gameObject.layer = LayerMask.NameToLayer("Weapon");
            ownerInventory[equipmentSlot] = nearestWeapon;
        }
        NetworkBehaviour weaponNetworkBehaviour = nearestWeapon.NetworkBehaviour;
        weaponNetworkBehaviour.NetworkObject.SetParent(this);
        weaponNetworkBehaviour.transform.localPosition = itemOffset;
        weaponNetworkBehaviour.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        PickUpWeaponRpc(weaponNetworkBehaviour);
    }

    [ObserversRpc]
    public void PickUpWeaponRpc(NetworkBehaviour weaponNetworkBehaviour) {
        Debug.Log("Client is now allowed to pickup");
        IWeapon weapon = (IWeapon)weaponNetworkBehaviour;
        int equipmentSlot = (int)weapon.Stats.Type;
        inventoriesSO.Inventories[OwnerId][equipmentSlot] = weapon;
        
        weaponNetworkBehaviour.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
    }
}