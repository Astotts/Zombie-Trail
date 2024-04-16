using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CameraSetup : NetworkBehaviour
{
    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();
        if (IsOwner)
            Camera.main.GetComponent<CameraTracking>().SetPlayer(transform);
    }
}
