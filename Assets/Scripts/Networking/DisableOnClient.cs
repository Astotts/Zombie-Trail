using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DisableOnClient : NetworkBehaviour
{

    public override void OnNetworkSpawn()
    {
        base.OnNetworkDespawn();
        if (!IsHost)
        {
            gameObject.SetActive(false);
        }
    }
}
