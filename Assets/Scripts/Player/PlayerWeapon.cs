using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Timeline;

public class PlayerWeapon : NetworkBehaviour
{

    [SerializeField] List<WeaponsClass> weapons = new();
    [SerializeField] List<AnimationGenerator> animators = new();

    NetworkVariable<int> currentWeapon = new();

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        SetWeaponServerRpc(0);
    }

    void Update()
    {
        if (!IsOwner)
            return;

        SetWeapon();

        PlayerAttack();
    }

    private void SetWeapon() {
        for (int i = 0; i < weapons.Capacity; i++) {
            if (Input.GetKey((KeyCode)(48 + i + 1)))
                SetWeaponServerRpc(i);
        }
    }

    private void PlayerAttack() {
        if(Input.GetMouseButton(0) && weapons[currentWeapon.Value].isActiveAndEnabled){
            weapons[currentWeapon.Value].Attack();
            PlayerAttackClientRpc();
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    void PlayerAttackClientRpc() {
        if (animators[currentWeapon.Value] != null)
            animators[currentWeapon.Value].StartAnimation();
    }
    
    [Rpc(SendTo.Server)]
    void SetWeaponServerRpc(int index) {
        currentWeapon.Value = index;
        SetWeaponClientRpc(index);
    }

    [Rpc(SendTo.ClientsAndHost)]
    void SetWeaponClientRpc(int index) {
        for(int i = 0; i <= 3; i++){
            if(i != currentWeapon.Value){
                if(animators[i] != null){
                    animators[i].StopAnimating();
                }
                weapons[i].gameObject.SetActive(false);
            }
        }
        weapons[index].gameObject.SetActive(true);
    }
}
