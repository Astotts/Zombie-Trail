using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

// InventoryController: Here to keep track of all the inventory in the game
public class InventoryController : MonoBehaviour
{
    [SerializeField] private float pickUpRadius;
    [SerializeField] private GameObject pickUpButtonGO;
    [SerializeField] private LayerMask layerToDetect;
    private PlayerInput playerInput;
    private int currentSlot = 0;
    private InputAction itemSwapAction;
    private InputAction itemPickUpAction;
    private InputAction itemDropAction;
    private GameObject closestGO = null;
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        itemSwapAction = playerInput.actions.FindAction("WeaponHotbar");
        itemSwapAction.Enable();
        itemSwapAction.performed += OnWeaponHotbarPressed;

        itemPickUpAction = playerInput.actions.FindAction("PickUpItem");
        itemDropAction.Enable();
        itemPickUpAction.performed += OnPickUpButtonPressed;

        itemDropAction = playerInput.actions.FindAction("Dropitem");
        itemDropAction.Enable();
        itemDropAction.performed += OnDropButtonPressed;
    }

    void OnDisable()
    {
        itemSwapAction.performed -= OnWeaponHotbarPressed;
        itemPickUpAction.performed -= OnPickUpButtonPressed;
        itemDropAction.performed -= OnDropButtonPressed;
    }

    public void HandlePickUpDistance()
    {
        Collider2D[] itemsAroundPlayer = Physics2D.OverlapCircleAll(transform.position, pickUpRadius, layerToDetect);
        float closestDistance = float.MaxValue;
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

    void HidePickUpButton()
    {
        pickUpButtonGO.SetActive(false);
    }
    void DisplayPickUpButton(Vector2 position)
    {
        pickUpButtonGO.SetActive(true);
        pickUpButtonGO.transform.position = position;
    }

    void OnPickUpButtonPressed(InputAction.CallbackContext context)
    {
        if (closestGO == null)
            return;

        InventoryManager.Instance.PickUpItem(closestGO.GetComponent<IItem>(), currentSlot);
    }

    void OnDropButtonPressed(InputAction.CallbackContext context)
    {
        InventoryManager.Instance.DropItem(currentSlot);
    }

    void OnWeaponHotbarPressed(InputAction.CallbackContext context)
    {
        // Get data for new item
        // Can't read value int here (maybe I can but I'm dumb)
        int nextSlot = (int) context.ReadValue<float>();

        if (!InventoryManager.Instance.SwapItem(currentSlot, nextSlot))
            return;

        // Update slots
        currentSlot = nextSlot;
    }
}
