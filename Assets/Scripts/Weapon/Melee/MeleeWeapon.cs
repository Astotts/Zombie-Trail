using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FishNet.Object;
using TreeEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class MeleeWeapon : NetworkBehaviour, IWeapon
{
    [SerializeField] private MeleeWeaponSO baseStats;
    
    public event EventHandler<WeaponAttackEventArgs> OnAttackEvent;
    public AbstractWeaponStats Stats => baseStats;
    public NetworkBehaviour NetworkBehaviour => this;
    public ReadOnlyCollection<IWeaponMod> WeaponMods => weaponMods.AsReadOnly();

    public Stat DamageFromStrength { get; private set; }
    public Stat KnockbackFromStrength { get; private set; }
    public Stat AttackSpeed { get; private set; }
    public Stat Cooldown { get; private set; }
    public Stat HitboxOffsetScale { get; set; }
    public Stat HitboxSizeScale { get; set; }

    private readonly List<IWeaponMod> weaponMods = new();

    void Start() {
        DamageFromStrength = new(baseStats.DamageFromStrength);
        KnockbackFromStrength = new(baseStats.KnockbackFromStrength);
        AttackSpeed = new(baseStats.AttackSpeed);
        Cooldown = new(baseStats.Cooldown);
        HitboxOffsetScale = new(1);
        HitboxSizeScale = new(1);
    }

    void OnDrawGizmos() {
        if (baseStats == null)
            return;

        Vector2 boxPos;
        Vector2 boxSize;
        
        if (HitboxOffsetScale == null || HitboxOffsetScale.Value == 0)
        {
            boxPos = baseStats.HitboxOffset;
            boxSize = baseStats.HitboxSize;
        }
        else
        {
            boxPos = baseStats.HitboxOffset * HitboxOffsetScale.Value;
            boxSize = baseStats.HitboxSize * HitboxSizeScale.Value;
        }
        Gizmos.color = Color.red;
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        Gizmos.matrix = rotationMatrix;
        Gizmos.DrawWireCube(boxPos, boxSize);
    }

    public void OnAttack(Player player)
    {
        if (!IsServerInitialized)
            return;
        
        OnAttackEvent?.Invoke(this, new WeaponAttackEventArgs(player, this));
        foreach (IWeaponMod mod in weaponMods) {
            mod.OnAttack(player, this);
        }

        Vector2 boxPos = transform.position + transform.rotation * (baseStats.HitboxOffset * HitboxOffsetScale.Value);
        Vector2 boxSize = baseStats.HitboxSize * HitboxSizeScale.Value;

        foreach (Collider2D collider in Physics2D.OverlapBoxAll(boxPos, boxSize, transform.rotation.eulerAngles.z))
        {
            if (collider.TryGetComponent(out IDamagable damagable))
                damagable.Damage(player.Strength.Value * DamageFromStrength.Value);
            
            if (collider.TryGetComponent(out IKnockable knockable))
            {
                Vector2 direction = collider.transform.position - transform.position;
                knockable.Knock(direction.normalized * player.Strength.Value * KnockbackFromStrength.Value);
            }
        }
    }

    public void OnUse(Player player)
    {
        foreach (IWeaponMod mod in weaponMods) {
            mod.OnUse(player, this);
        }
    }

    public void OnDrop(Player player)
    {
        // Sound effect
    }

    public void OnPickUp(Player player)
    {
        // Play sound effect or something
    }

    public void OnSwapIn(Player player)
    {
        foreach (IWeaponMod mod in weaponMods) {
            mod.OnSwapIn(player, this);
        }
    }

    public void OnSwapOut(Player player)
    {
        foreach (IWeaponMod mod in weaponMods) {
            mod.OnSwapOut(player, this);
        }
    }
}