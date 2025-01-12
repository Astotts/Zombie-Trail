using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ParticleNetworkDespawn : NetworkBehaviour
{
    [SerializeField] NetworkObject ownerNetworkObject;
    [SerializeField] ParticlePrefabSO particlePrefabSO;

    void OnValidate()
    {
        if (ownerNetworkObject == null)
            ownerNetworkObject = GetComponent<NetworkObject>();
    }

    public void OnParticleSystemEnd()
    {
        if (!IsServer)
            return;

        NetworkObjectPool.Singleton.ReturnNetworkObject(ownerNetworkObject, particlePrefabSO.Prefab);
    }
}
