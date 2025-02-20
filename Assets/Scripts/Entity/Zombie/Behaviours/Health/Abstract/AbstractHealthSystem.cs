using Unity.Netcode;
using UnityEditor.ShaderGraph;
using UnityEngine;

public abstract class AbstractHealthSystem : NetworkBehaviour, IDamageable
{
    public abstract HealthSystemStats Stats { get; }
    public abstract void Damage(float amount);
}