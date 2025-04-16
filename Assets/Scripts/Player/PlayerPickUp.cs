using System;
using System.Collections;
using FishNet.Connection;
using FishNet.Object;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.MPE;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPickUp : NetworkBehaviour
{
    [SerializeField] PlayerInventoriesSO inventoriesSO;
    [SerializeField] float pickUpRange;
    [SerializeField] Vector2 itemOffset;
    [SerializeField] float pickUpInterval;
    [SerializeField] GameObject pickUpButton;

    InputAction pickUpAction;

    Coroutine pickUpCoroutine;

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (!IsOwner)
            return;

        pickUpAction = InputSystem.actions.FindAction("PickUp");
        pickUpAction.performed += OnPickUpPerformed;
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        
        if (!IsOwner)
            return;

        pickUpAction.performed -= OnPickUpPerformed;
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
        LayerMask mask = LayerMask.NameToLayer("Weapon");
        // Gotta use 1 << to make the layer bitmask instead
        foreach (Collider2D collider in Physics2D.OverlapCircleAll(pickUpCenter, pickUpRange, 1 << mask))
        {
            Debug.Log(collider.gameObject.name);
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

    void DropWeapon(IWeapon weapon)
    {
        NetworkBehaviour weaponBehaviour = weapon.NetworkBehaviour;
        weaponBehaviour.gameObject.SetActive(true);
        weaponBehaviour.transform.localPosition = Vector3.zero;
        weaponBehaviour.NetworkObject.UnsetParent();
        weaponBehaviour.gameObject.layer = LayerMask.NameToLayer("Weapon");
        Debug.Log("Dropped");
    }

    [ObserversRpc]
    void DropWeaponObserverRpc(NetworkBehaviour weaponBehaviour)
    {
        weaponBehaviour.transform.localPosition = Vector3.zero;
        weaponBehaviour.NetworkObject.UnsetParent();
        weaponBehaviour.gameObject.layer = LayerMask.NameToLayer("Weapon");
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

        // Drop if the current slot have a weapon
        if (currentWeapon != null)
        {
            DropWeapon(currentWeapon);
        }
        
        PickUpWeapon(nearestWeapon);

        // Send only the new weapon to clients
        if (inventoriesSO.CurrentSlot[OwnerId] == equipmentSlot)
        {
            Debug.Log("Current weapon should be true");
            PickUpWeaponObserverRpc(nearestWeapon.NetworkBehaviour);
        }
        // Send the new weapon along with new slot
        else
        {
            Debug.Log("Current weapon should be false");
            int currentSlot = inventoriesSO.CurrentSlot[OwnerId];
            IWeapon currentSlotWeapon = inventoriesSO.Inventories[OwnerId][currentSlot];
            if (currentSlotWeapon != null)
            {
                NetworkBehaviour currentSlotWeaponBehaviour = currentSlotWeapon.NetworkBehaviour;
                currentSlotWeaponBehaviour.gameObject.SetActive(false);
                PickUpWeaponObserverRpc(currentSlotWeaponBehaviour, nearestWeapon.NetworkBehaviour, equipmentSlot);
            }
            else
            {
                PickUpWeaponObserverRpc(nearestWeapon.NetworkBehaviour, equipmentSlot);
            }
        }
    }

    void PickUpWeapon(IWeapon weapon)
    {
        NetworkBehaviour nearestWeaponBehaviour = weapon.NetworkBehaviour;
        nearestWeaponBehaviour.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        nearestWeaponBehaviour.NetworkObject.SetParent(this);
        nearestWeaponBehaviour.transform.localPosition = itemOffset;
    }

    [ObserversRpc]
    public void PickUpWeaponObserverRpc(NetworkBehaviour weaponNetworkBehaviour) {
        Debug.Log("Clients is picking up");
        if (IsOwner)
        {
            // Only owner of this player need this
            IWeapon weapon = (IWeapon)weaponNetworkBehaviour;
            int equipmentSlot = (int)weapon.Stats.Type;
            inventoriesSO.Inventories[OwnerId][equipmentSlot] = weapon;
        }
        
        weaponNetworkBehaviour.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        weaponNetworkBehaviour.NetworkObject.SetParent(this);
        weaponNetworkBehaviour.transform.localPosition = itemOffset;
    }


    [ObserversRpc]
    public void PickUpWeaponObserverRpc(NetworkBehaviour newWeaponBehaviour, int newEquipSlot) {
        Debug.Log("Clients is picking up");
        if (IsOwner)
        {
            // Only owner of this player need this
            IWeapon weapon = (IWeapon)newWeaponBehaviour;
            int equipmentSlot = (int)weapon.Stats.Type;
            inventoriesSO.Inventories[OwnerId][equipmentSlot] = weapon;
            inventoriesSO.CurrentSlot[OwnerId] = newEquipSlot;
        }
        newWeaponBehaviour.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        newWeaponBehaviour.NetworkObject.SetParent(this);
        newWeaponBehaviour.transform.localPosition = itemOffset;
    }

    [ObserversRpc]
    public void PickUpWeaponObserverRpc(NetworkBehaviour currentWeaponBehaviour, NetworkBehaviour newWeaponBehaviour, int newEquipSlot) {
        Debug.Log("Clients is picking up");
        if (IsOwner)
        {
            // Only owner of this player need this
            IWeapon weapon = (IWeapon)newWeaponBehaviour;
            int equipmentSlot = (int)weapon.Stats.Type;
            inventoriesSO.Inventories[OwnerId][equipmentSlot] = weapon;
            inventoriesSO.CurrentSlot[OwnerId] = newEquipSlot;
        }
        currentWeaponBehaviour.gameObject.SetActive(false);
        
        newWeaponBehaviour.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        newWeaponBehaviour.NetworkObject.SetParent(this);
        newWeaponBehaviour.transform.localPosition = itemOffset;
    }
}