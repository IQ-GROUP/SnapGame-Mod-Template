using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class SyncBackbling : NetworkBehaviour
{
    [Tooltip("Assign Pizza backbling GameObject")]
    [SerializeField] private GameObject pizza;
    
    [Tooltip("Assign Pizza backbling GameObject")]
    [SerializeField] private GameObject tec9Backpack;

    [Tooltip("Assign Gun backbling GameObject")]
    [SerializeField] private GameObject gun;

    [Tooltip("Assign Racket backbling GameObject")]
    [SerializeField] private GameObject racket;

    [Tooltip("Assign Cart backbling GameObject")]
    [SerializeField] private GameObject cart;
    
    // List to keep track of all hat GameObjects for easier disabling
    private List<GameObject> allBackblingGameObjects = new List<GameObject>();
    
    // NetworkVariable to sync the chosen hat across all clients
    private NetworkVariable<PlayerBackbling> syncedPlayerBackbling = new NetworkVariable<PlayerBackbling>(
        PlayerBackbling.None, 
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Owner);

    // Enum to represent player hats
    public enum PlayerBackbling
    {
        None = -1,
        Pizza = 0,
        Tec9Backpack = 1,
        Gun = 2,
        Racket = 3,
        Cart = 4
    }

    // Dictionary to map string hat names to enum values
    private static readonly Dictionary<string, PlayerBackbling> BackblingNameToEnum = new Dictionary<string, PlayerBackbling>
    {
        { "Pizza", PlayerBackbling.Pizza },
        { "Tec9Backpack", PlayerBackbling.Tec9Backpack },
        { "Gun", PlayerBackbling.Gun },
        { "Racket", PlayerBackbling. Racket},
        { "Cart", PlayerBackbling. Cart}
    };

    void Awake()
    {
        // Check if all necessary hat GameObjects are assigned
        if (pizza == null)
        {
            Debug.LogError("BackblingSync: Pizza backbling GameObject is not assigned!");
        }
        
        // Initially disable all hat objects
        DisableAllHats();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // Only the owner should set the initial value
        if (IsOwner)
        {
            string backblingChoice = PlayerPrefs.GetString("Backbling", "");
            
            // Convert the string to enum
            PlayerBackbling playerBackbling = PlayerBackbling.None; // Default is no hat
            if (BackblingNameToEnum.TryGetValue(backblingChoice, out PlayerBackbling backbling))
            {
                playerBackbling = backbling;
            }
            else
            {
                Debug.LogWarning($"PlayerHatManager: Invalid hat choice '{backblingChoice}', using no hat as default.");
            }
            
            // Set the networked value
            syncedPlayerBackbling.Value = playerBackbling;
        }

        // Everyone subscribes to the value change event
        syncedPlayerBackbling.OnValueChanged += OnPlayerHatChanged;
        
        // Apply the initial hat
        ApplyHat(syncedPlayerBackbling.Value);
    }

    public override void OnNetworkDespawn()
    {
        // Unsubscribe when despawning
        syncedPlayerBackbling.OnValueChanged -= OnPlayerHatChanged;
        base.OnNetworkDespawn();
    }

    private void OnPlayerHatChanged(PlayerBackbling previousValue, PlayerBackbling newValue)
    {
        ApplyHat(newValue);
    }

    private void DisableAllHats()
    {
        foreach (var backblingObject in allBackblingGameObjects)
        {
            if (backblingObject != null)
            {
                backblingObject.SetActive(false);
            }
        }
    }

    private void ApplyHat(PlayerBackbling backbling)
    {
        // First disable all hats
        DisableAllHats();
        
        // If no hat selected, just leave all disabled
        if (backbling == PlayerBackbling.None) return;
        
        switch (backbling)
        {
            
            case PlayerBackbling.Pizza:
                if (pizza != null)
                {
                    pizza.SetActive(true);
                }
                break;
            case PlayerBackbling.Tec9Backpack:
                if (tec9Backpack != null)
                {
                    tec9Backpack.SetActive(true);
                }
                break;
            case PlayerBackbling.Gun:
                if (gun != null)
                {
                    gun.SetActive(true);
                }
                break;
            case PlayerBackbling.Racket:
                if (racket != null)
                {
                    racket.SetActive(true);
                }
                break;
            case PlayerBackbling.Cart:
                if (cart != null)
                {
                    cart.SetActive(true);
                }
                break;
                
            
            default:
                Debug.LogError($"PlayerHatManager: Unhandled hat type: {backbling}");
                break;
        }
    }
}