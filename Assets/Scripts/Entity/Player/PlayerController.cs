using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] float moveSpeed = 1f;
    PlayerInput playerInput;       // Use this to pull all the values needed for the inputs
    PlayerControls playerControls;          // Input Action Asset - This allows the controls for the player. Check the file out to see the set up. (This will be obsolete, will use PlayerInput)
    Vector2 movement;                       // Controls the player movement
    Vector2 prevMovement = Vector2.zero;
    int weaponSelected;                     // Keeps track of what weapon is selected

    Rigidbody2D rb;                         // Controls the player rigidbody

    //----------------------------------------

    int selectedWeapon;

    [SerializeField] List<WeaponsClass> weapons;
    [SerializeField] List<AnimationGenerator> animators;

    //----------------------------------------

    [SerializeField] Animator animator;

    //----------------------------------------

    [SerializeField] private AudioClip[] sounds;
    [SerializeField] private AudioSource audioSource;
    private float walkingElapsed;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            enabled = false;
            return;
        }
        base.OnNetworkSpawn();
    }
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        if (weapons == null)
        {
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
        for (int i = 0; i <= 1; i++)
        {
            if (weapons[i] is RangedWeapons ranged)
            {
                ranged.reloading = false;
                ranged.SetOpacity(0);
            }
            weapons[i].ui.SetActive(false);
            weapons[i].gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        PlayerInputs();

        for (int i = 0; i <= 1; i++)
        {
            if (i != selectedWeapon)
            {
                if (animators[i] != null)
                {
                    animators[i].StopAnimating();
                }
                if (weapons[i] is RangedWeapons ranged)
                {
                    ranged.reloading = false;
                    ranged.SetOpacity(0);
                }
                weapons[i].ui.SetActive(false);
                weapons[i].gameObject.SetActive(false);
            }
        }
        weapons[selectedWeapon].gameObject.SetActive(true);
        weapons[selectedWeapon].ui.SetActive(true);

        if (Input.GetMouseButton(0))
        {
            weapons[selectedWeapon].Attack();
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    void PlayerInputs()
    {
        // Get the x/y values from the "Input Action" asset, which has the keys assigned x +/-, y +/-
        //movement = playerInput.actions                      //.Movement.Move.ReadValue<Vector2>();
        //movement = playerInput.
        movement = playerControls.Movement.Move.ReadValue<Vector2>();

        walkingElapsed += Time.deltaTime;

        if(movement != prevMovement || (audioSource.clip != null && audioSource.clip.length < walkingElapsed)){
            audioSource.pitch = UnityEngine.Random.Range(0.7f, 1.1f);
            audioSource.clip = sounds[UnityEngine.Random.Range(0, 3)];
            walkingElapsed = 0f;
            if(movement != Vector2.zero){
                audioSource.Play();
            }
        }

        if (movement == Vector2.zero)
        {
            audioSource.Stop();
            animator.Play("Idle");
        }
        else if (movement.x < 0f)
        {
            animator.Play("Player-Walk-Left");
        }
        else if (movement.x > 0f)
        {
            animator.Play("Player-Walk-Right");
        }
        else if (movement.y > 0f)
        {
            animator.Play("Player-Walk-Up");
        }
        else if (movement.y < 0f)
        {
            animator.Play("Player-Walk-Down");
        }
        //Debug.Log("Movement x: " + movement.x);
        //Debug.Log("Movement y: " + movement.y);

        //Get Selected Slot
        if ((0 < playerControls.Equipment.FirstWeapon.ReadValue<float>())) selectedWeapon = 0;
        if ((0 < playerControls.Equipment.SecondWeapon.ReadValue<float>())) selectedWeapon = 1;

        prevMovement = playerControls.Movement.Move.ReadValue<Vector2>();
    }

    void MovePlayer()
    {
        transform.position += (Vector3)movement * (moveSpeed * Time.fixedDeltaTime);  // Current position + where to move * (the movement speed * timing of movement)
    }
}
