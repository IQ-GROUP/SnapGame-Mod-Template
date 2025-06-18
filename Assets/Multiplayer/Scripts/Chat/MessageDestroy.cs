using Unity.Netcode;
using UnityEngine;

public class MessageDestroy : NetworkBehaviour
{
    public void DestroySelf(){
        DestroySelfServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void DestroySelfServerRpc(){
        gameObject.GetComponent<NetworkObject>().Despawn(true);
    }
}