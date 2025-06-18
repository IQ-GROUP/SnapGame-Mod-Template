using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class NetworkBuildingManager : NetworkBehaviour
{
    public GameObject[] buildingPrefabs; // Assign prefabs in Inspector

public AudioClip placementSound; // Assign in inspector
public AudioSource audioSource;  // Assign in inspector or dynamically

[ClientRpc]
void PlayPlacementSoundClientRpc()
{
    if (audioSource != null && placementSound != null)
    {
        audioSource.PlayOneShot(placementSound);
    }
}

    public void InstantiateBuildingObject(int prefabIndex, Vector3 position, Quaternion rotation, Vector3 scale, bool isKinematic)
    {
        InstantiateBuildingObjectServerRpc(prefabIndex, position, rotation, scale, isKinematic);
    }

    [ServerRpc(RequireOwnership = false)]
    void InstantiateBuildingObjectServerRpc(int prefabIndex, Vector3 position, Quaternion rotation, Vector3 scale, bool isKinematic)
    {
        Debug.Log("NetworkBuildingManager: " + prefabIndex);
        if (prefabIndex < 0 || prefabIndex >= buildingPrefabs.Length)
        {
            Debug.LogError("Invalid prefab index!");
            return;
        }

        var instance = Instantiate(buildingPrefabs[prefabIndex], position, rotation);
        instance.transform.localScale = scale;

        //Set kinematic state of the Rigidbody
        // Rigidbody rb = instance.GetComponent<Rigidbody>();
        // if (rb != null)
        // {
        //     rb.isKinematic = isKinematic;
        //     if(isKinematic){
        //         Destroy(rb);
        //     }
        // }

        instance.GetComponent<NetworkObject>().Spawn(true);
        PlayPlacementSoundClientRpc();
    }
    
    // New method to destroy networked objects
    public void DestroyBuildingObject(ulong networkObjectId)
    {
        DestroyBuildingObjectServerRpc(networkObjectId);
    }
    
    [ServerRpc(RequireOwnership = false)]
    void DestroyBuildingObjectServerRpc(ulong networkObjectId)
    {
        // Check if the dictionary contains the key first
        if (NetworkManager.SpawnManager.SpawnedObjects.ContainsKey(networkObjectId))
        {
            NetworkObject networkObject = NetworkManager.SpawnManager.SpawnedObjects[networkObjectId];
            
            if (networkObject != null)
            {
                Debug.Log($"Destroying networked object with ID: {networkObjectId}");
                networkObject.Despawn(true);
            }
        }
        else
        {
            Debug.LogWarning($"Failed to find networked object with ID: {networkObjectId}");
        }
    }
}