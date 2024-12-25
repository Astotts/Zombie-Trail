using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

// InventoryController: Here to keep track of all the inventory in the game
public class InventoryController : MonoBehaviour
{
    [SerializeField] private float pickUpRadius;
    [SerializeField] private GameObject pickUpButtonGO;
    [SerializeField] private LayerMask layerToDetect;
    private PlayerControls playerControls;
    private int currentSlot = 0;
    private GameObject closestGO = null;
    void Awake()
    {
        playerControls = new PlayerControls();

        playerControls.Equipment.WeaponHotbar.performed += OnWeaponHotbarPressed;
        playerControls.Equipment.PickUpItem.performed += OnPickUpButtonPressed;
        playerControls.Equipment.DropItem.performed += OnDropButtonPressed;
        playerControls.Equipment.ItemLeftClick.started += OnItemLeftClickPressed;
        playerControls.Equipment.ItemLeftClick.canceled += OnItemLeftClickReleased;
        playerControls.Equipment.ItemRightClick.started += OnItemRightClickPressed;
        playerControls.Equipment.ItemRightClick.canceled += OnItemRightClickReleased;
    }

    void OnEnable()
    {
        playerControls.Equipment.Enable();
    }

    void OnDisable()
    {
        playerControls.Equipment.Disable();
    }

    void OnDestroy()
    {
        playerControls.Equipment.WeaponHotbar.performed -= OnWeaponHotbarPressed;
        playerControls.Equipment.PickUpItem.performed -= OnPickUpButtonPressed;
        playerControls.Equipment.DropItem.performed -= OnDropButtonPressed;
        playerControls.Equipment.ItemLeftClick.started -= OnItemLeftClickPressed;
        playerControls.Equipment.ItemLeftClick.canceled -= OnItemLeftClickReleased;
        playerControls.Equipment.ItemRightClick.started -= OnItemRightClickPressed;
        playerControls.Equipment.ItemRightClick.canceled -= OnItemRightClickReleased;
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

    void OnItemLeftClickPressed(InputAction.CallbackContext context)
    {
        InventoryManager.Instance.LeftCLickItemPressed(currentSlot);
    }
    void OnItemLeftClickReleased(InputAction.CallbackContext context)
    {
        InventoryManager.Instance.LeftCLickItemReleased(currentSlot);
    }

    void OnItemRightClickPressed(InputAction.CallbackContext context)
    {
        InventoryManager.Instance.RightClickItemPressed(currentSlot);
    }
    void OnItemRightClickReleased(InputAction.CallbackContext context)
    {
        InventoryManager.Instance.RightClickItemReleased(currentSlot);
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
        int nextSlot = (int)context.ReadValue<float>();

        if (!InventoryManager.Instance.SwapItem(currentSlot, nextSlot))
            return;

        // Update slots
        currentSlot = nextSlot;
    }
}
