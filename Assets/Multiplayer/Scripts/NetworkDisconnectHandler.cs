using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkDisconnectHandler : MonoBehaviour
{
    [SerializeField] private string sceneToLoadOnDisconnect = "MultiplayerTests";
    
    private void Awake()
    {
        // Subscribe to network events
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleDisconnect;
        NetworkManager.Singleton.OnServerStopped += HandleServerStopped;
    }

    private void OnDestroy()
    {
        // Clean up subscriptions if NetworkManager still exists
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleDisconnect;
            NetworkManager.Singleton.OnServerStopped -= HandleServerStopped;
        }
    }

    private void HandleDisconnect(ulong clientId)
    {
        // Only reload scene if this is our own client disconnecting
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            Debug.Log("Local client disconnected, reloading scene");
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            ReloadScene();
        }
    }

    private void HandleServerStopped(bool wasShutdown)
    {
        Debug.Log("Server stopped, reloading scene");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        ReloadScene();
    }

    private void ReloadScene()
    {
        // Make sure we're not in a networked state anymore
        if (NetworkManager.Singleton.IsConnectedClient || NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.Shutdown();
        }
        
        // Load the specified scene
        SceneManager.LoadScene(sceneToLoadOnDisconnect);
    }
    
    // Call this function when you want to manually disconnect and reload the scene
    public void DisconnectAndRestart()
    {
        Debug.Log("Manual disconnect requested, reloading scene");
        
        // If we're a host or server, shut down the server
        if (NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.Shutdown();
        }
        // If we're just a client, disconnect from the server
        else if (NetworkManager.Singleton.IsConnectedClient)
        {
            NetworkManager.Singleton.Shutdown();
        }
        // If we're not connected to anything, just reload the scene
        else
        {
            ReloadScene();
        }
        
        // Note: The scene reload will happen via the HandleDisconnect or HandleServerStopped callbacks
    }
}