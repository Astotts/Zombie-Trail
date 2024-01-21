using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 1f;
    PlayerControls playerControls;          // Input Action Asset - This allows the controls for the player. Check the file out to see the set up.
    Vector2 movement;                       // Controls the player movement
    Rigidbody2D rb;                         // Controls the player rigidbody

    //----------------------------------------

    int selectedWeapon;

    [SerializeField] List<WeaponsClass> weapons;

    //----------------------------------------

    [SerializeField] Animator animator;


    private void Awake()
    {
        if(weapons == null){
            weapons = new List<WeaponsClass>();
        }
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
        
        for(int i = 0; i <= 2; i++){
            if(i != selectedWeapon){
                weapons[i].gameObject.SetActive(false);
            }
        }
        weapons[selectedWeapon].gameObject.SetActive(true);

        if(Input.GetMouseButtonDown(0)){
            weapons[selectedWeapon].Attack();
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    void PlayerInput()
    {
        // Get the x/y values from the "Input Action" asset, which has the keys assigned x +/-, y +/-
        movement = playerControls.Movement.Move.ReadValue<Vector2>();

        if(movement == Vector2.zero){
            animator.Play("Idle");
        }else if(movement.x < 0f){
            animator.Play("Player-Walk-Left");
        }else if(movement.x > 0f){
            animator.Play("Player-Walk-Right");
        }else if(movement.y > 0f){
            animator.Play("Player-Walk-Up");
        }else if(movement.y < 0f){
            animator.Play("Player-Walk-Down");
        }
        //Debug.Log("Movement x: " + movement.x);
        //Debug.Log("Movement y: " + movement.y);

        //Get Selected Slot
        if(Input.GetKeyDown("1")) selectedWeapon = 0;
        if(Input.GetKeyDown("2")) selectedWeapon = 1;
        if(Input.GetKeyDown("3")) selectedWeapon = 2;
        if(Input.GetKeyDown("4")) selectedWeapon = 3; 
    }

    void MovePlayer()
    {
        rb.MovePosition(rb.position + movement * (moveSpeed * Time.fixedDeltaTime));  // Current position + where to move * (the movement speed * timing of movement)
    }
}
