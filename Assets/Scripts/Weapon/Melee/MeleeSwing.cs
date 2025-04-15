using System;
using System.Collections;
using System.Security.Cryptography;
using FishNet.Object;
using LiteNetLib;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class MeleeSwing : NetworkBehaviour
{
    public static readonly int ATTACK_SPEED_RATIO = 10;
    [SerializeField] private MeleeWeapon weapon;
    [SerializeField] private float MinAngle;
    [SerializeField] private float MaxAngle;

    bool currentMax = false;

    void OnEnable() {
        weapon.OnAttackEvent += OnMeleeAttack;
    }

    void OnDisable() {
        weapon.OnAttackEvent -= OnMeleeAttack;
    }

    private void OnMeleeAttack(object sender, WeaponAttackEventArgs e)
    {
        MeleeWeaponSO stats = (MeleeWeaponSO) e.Weapon.Stats;
        RpcRotateWeapon(stats.AttackSpeed);
    }

    [ObserversRpc]
    private void RpcRotateWeapon(int attackSpeed) {

        StartCoroutine(RotateWeaponAnimation(attackSpeed));
    }

    IEnumerator RotateWeaponAnimation(int attackSpeed) {
        float swingTime = ATTACK_SPEED_RATIO / attackSpeed;
        float elapsed = 0;
        float fromAngle = currentMax ? MaxAngle : MinAngle;
        float toAngle = currentMax ? MinAngle : MaxAngle;
        while (elapsed <= swingTime)
        {
            float zAngle = Mathf.Lerp(fromAngle, toAngle, elapsed / swingTime);
            transform.eulerAngles = new Vector3(0, 0, zAngle);
            elapsed += Time.deltaTime;
            yield return null;
        }
        currentMax = !currentMax;
    }
}
