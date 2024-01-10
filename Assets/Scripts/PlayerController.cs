using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 1f;

    PlayerControls playerControls;          // Input Action Asset - This allows the controls for the player. Check the file out to see the set up.
    Vector2 movement;                       // Controls the player movement
    Rigidbody2D rb;                         // Controls the player rigidbody

    private void Awake()
    {
        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();

    }

    // Enable the controls for the players
    private void OnEnable()
    {
        playerControls.Enable();
    }

    // Game Over or Pause. Not in use yets
    private void OnDisable()
    {
        playerControls.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PlayerInput();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    void PlayerInput()
    {
        // Get the x/y values from the "Input Action" asset, which has the keys assigned x +/-, y +/-
        movement = playerControls.Movement.Move.ReadValue<Vector2>();

        //Debug.Log("Movement x: " + movement.x);
        //Debug.Log("Movement y: " + movement.y);
    }

    void MovePlayer()
    {
        rb.MovePosition(rb.position + movement * (moveSpeed * Time.fixedDeltaTime));  // Current position + where to move * (the movement speed * timing of movement)
    }
}
