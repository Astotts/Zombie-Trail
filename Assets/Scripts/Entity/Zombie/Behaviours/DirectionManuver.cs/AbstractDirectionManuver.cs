using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

[RequireComponent(typeof(NetworkTransform))]
public abstract class AbstractDirectionManuver : NetworkBehaviour
{
    [field: SerializeField] public virtual BaseDirectionManuverStats Stats { get; private set; }
    public Transform Target { get; set; }
    abstract public void RotateTowardTarget();
    abstract public Transform FindNearestTarget();
}