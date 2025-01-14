using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    PlayerInput playerInput;       // Use this to pull all the values needed for the inputs
    PlayerControls playerControls;          // Input Action Asset - This allows the controls for the player. Check the file out to see the set up. (This will be obsolete, will use PlayerInput)
    Vector2 input;                       // Controls the player movement
    Vector2 prevMovement = Vector2.zero;
    [SerializeField] PlayerMovement playerMovement;


    //----------------------------------------

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
        playerControls = new PlayerControls();
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

    // Update is called once per frame
    void Update()
    {
        PlayerInputs();
    }

    void PlayerInputs()
    {
        // Get the x/y values from the "Input Action" asset, which has the keys assigned x +/-, y +/-
        //movement = playerInput.actions                      //.Movement.Move.ReadValue<Vector2>();
        //movement = playerInput.
        input = playerControls.Movement.Move.ReadValue<Vector2>();

        walkingElapsed += Time.deltaTime;

        if (input != prevMovement || (audioSource.clip != null && audioSource.clip.length < walkingElapsed))
        {
            audioSource.pitch = UnityEngine.Random.Range(0.7f, 1.1f);
            audioSource.clip = sounds[UnityEngine.Random.Range(0, 3)];
            walkingElapsed = 0f;
            if (input != Vector2.zero)
            {
                audioSource.Play();
            }
        }

        if (input == Vector2.zero)
        {
            audioSource.Stop();
            animator.Play("Idle");
        }
        else if (input.x < 0f)
        {
            animator.Play("Player-Walk-Left");
        }
        else if (input.x > 0f)
        {
            animator.Play("Player-Walk-Right");
        }
        else if (input.y > 0f)
        {
            animator.Play("Player-Walk-Up");
        }
        else if (input.y < 0f)
        {
            animator.Play("Player-Walk-Down");
        }
        //Debug.Log("Movement x: " + movement.x);
        //Debug.Log("Movement y: " + movement.y);

        prevMovement = playerControls.Movement.Move.ReadValue<Vector2>();

        playerMovement.Input(input);
    }
}
