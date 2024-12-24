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
    private InputAction itemLeftClick;
    private InputAction itemRightClick;
    private GameObject closestGO = null;
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        itemSwapAction = playerInput.actions.FindAction("WeaponHotbar");
        itemSwapAction.Enable();
        itemSwapAction.performed += OnWeaponHotbarPressed;

        itemPickUpAction = playerInput.actions.FindAction("PickUpItem");
        itemPickUpAction.Enable();
        itemPickUpAction.performed += OnPickUpButtonPressed;

        itemDropAction = playerInput.actions.FindAction("Dropitem");
        itemDropAction.Enable();
        itemDropAction.performed += OnDropButtonPressed;

        itemLeftClick = playerInput.actions.FindAction("ItemLeftClick");
        itemLeftClick.Enable();
        itemLeftClick.performed += OnItemLeftClick;
    }

    void OnDisable()
    {
        itemSwapAction.performed -= OnWeaponHotbarPressed;
        itemPickUpAction.performed -= OnPickUpButtonPressed;
        itemDropAction.performed -= OnDropButtonPressed;
        itemLeftClick.performed -= OnItemLeftClick;
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

    void OnItemLeftClick(InputAction.CallbackContext context)
    {
        InventoryManager.Instance.LeftCLickItem(currentSlot);
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
