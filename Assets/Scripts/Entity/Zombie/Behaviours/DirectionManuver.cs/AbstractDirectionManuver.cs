using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

[RequireComponent(typeof(NetworkTransform))]
public abstract class AbstractDirectionManuver : NetworkBehaviour
{
    [field: SerializeField] public virtual BaseDirectionManuverStats Stats { get; private set; }
    public Transform Target { get; set; }
    public abstract void RotateTowardTarget();
    public abstract Transform FindNearestTarget();
    public abstract Vector2 GetDirection();
}