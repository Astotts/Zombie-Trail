using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions.Comparers;
using UnityEngine.Rendering.Universal.Internal;

public class PlayerMovement : NetworkBehaviour
{   
    [SerializeField] float moveSpeed;
    [SerializeField] Animator animator;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner)
            return;
        MovePlayer();
    }

    void MovePlayer()
    {
        float speedDelta = moveSpeed * Time.deltaTime;

        float newPosX = speedDelta * Input.GetAxisRaw("Horizontal");
        float newPosY = speedDelta * Input.GetAxisRaw("Vertical");

        if (newPosX == 0 && newPosY == 0)
            animator.Play("Idle");
        else if(newPosX < 0f)
            animator.Play("Player-Walk-Left");
        else if(newPosX > 0f)
            animator.Play("Player-Walk-Right");
        else if(newPosY > 0f)
            animator.Play("Player-Walk-Up");
        else if(newPosY < 0f)
            animator.Play("Player-Walk-Down");

        if (newPosX != 0 && newPosY != 0) {
            newPosX /= 1.5f;
            newPosY /= 1.5f;
        }
        

        transform.Translate(newPosX, newPosY, 0f);
    }
}
