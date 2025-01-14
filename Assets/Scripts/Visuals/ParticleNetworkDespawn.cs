using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ParticleNetworkDespawn : NetworkBehaviour
{
    [SerializeField] NetworkObject ownerNetworkObject;

    void OnValidate()
    {
        if (ownerNetworkObject == null)
            ownerNetworkObject = GetComponent<NetworkObject>();
    }

    public void OnParticleSystemStopped()
    {
        if (!IsServer)
            return;

        ownerNetworkObject.Despawn();
    }
}
