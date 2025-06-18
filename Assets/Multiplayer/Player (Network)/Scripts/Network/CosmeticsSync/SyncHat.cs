using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class SyncHat : NetworkBehaviour
{
    [Tooltip("Assign Duck hat GameObject")]
    [SerializeField] private GameObject duckHat;
    
    [Tooltip("Assign AppleCap hat GameObject")]
    [SerializeField] private GameObject appleCapHat;
    
    [Tooltip("Assign first part of BlackSpecial hat")]
    [SerializeField] private GameObject blackSpecialHatPart1;

    [Tooltip("Assign PreOrderHat hat GameObject")]
    [SerializeField] private GameObject preOrderHat;

    [Tooltip("Assign PartyHat hat GameObject")]
    [SerializeField] private GameObject partyHat;
    
    [Tooltip("Assign Pot hat GameObject")]
    [SerializeField] private GameObject potHat;
    
    [Tooltip("Assign Shark Hat GameObject")]
    [SerializeField] private GameObject sharkHat;
    
    [Tooltip("Assign Crown Hat GameObject")]
    [SerializeField] private GameObject crownHat;

    [Tooltip("Assign Dog Ears Hat GameObject")]
    [SerializeField] private GameObject dogEars;

    [Tooltip("Assign Golden Pot Hat Hat GameObject")]
    [SerializeField] private GameObject goldenPotHat;

    [Tooltip("Assign Horns Hat GameObject")]
    [SerializeField] private GameObject horns;

    [Tooltip("Assign Admin Horns Hat GameObject")]
    [SerializeField] private GameObject adminHorns;

    [Tooltip("Assign Glasses Hat GameObject")]
    [SerializeField] private GameObject glasses;

    [Tooltip("Assign Summer Hat GameObject")]
    [SerializeField] private GameObject summerHat;

    // List to keep track of all hat GameObjects for easier disabling
    private List<GameObject> allHatObjects = new List<GameObject>();
    
    // NetworkVariable to sync the chosen hat across all clients
    private NetworkVariable<PlayerHat> syncedPlayerHat = new NetworkVariable<PlayerHat>(
        PlayerHat.None, 
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Owner);

    // Enum to represent player hats
    public enum PlayerHat
    {
        None = -1,
        Duck = 0,
        AppleCap = 1,
        BlackSpecial = 2,
        PreOrderHat = 3,
        PartyHat = 4,
        PotHat = 5,
        SharkHat = 6,
        CrownHat = 7,
        DogEars = 8,
        GoldenPotHat = 9,
        Horns = 10,
        AdminHorns = 11,
        Glasses = 12,
        SummerHat = 13
    }

    // Dictionary to map string hat names to enum values
    private static readonly Dictionary<string, PlayerHat> HatNameToEnum = new Dictionary<string, PlayerHat>
    {
        { "Duck", PlayerHat.Duck },
        { "AppleCap", PlayerHat.AppleCap },
        { "BlackSpecial", PlayerHat.BlackSpecial },
        { "PreOrderHat", PlayerHat.PreOrderHat },
        { "PartyHat", PlayerHat.PartyHat },
        { "PotHat", PlayerHat.PotHat },
        { "SharkHat", PlayerHat.SharkHat },
        { "CrownHat", PlayerHat.CrownHat },
        { "DogEars", PlayerHat.DogEars },
        { "GoldenPotHat", PlayerHat.GoldenPotHat },
        { "Horns", PlayerHat.Horns },
        { "AdminHorns", PlayerHat.AdminHorns },
        { "Glasses", PlayerHat.Glasses },
        { "SummerHat", PlayerHat.SummerHat }
    };

    void Awake()
    {
        // Check if all necessary hat GameObjects are assigned
        if (duckHat == null)
        {
            Debug.LogError("PlayerHatManager: Duck hat GameObject is not assigned!");
        }
        else
        {
            allHatObjects.Add(duckHat);
        }
        
        if (appleCapHat == null)
        {
            Debug.LogError("PlayerHatManager: AppleCap hat GameObject is not assigned!");
        }
        else
        {
            allHatObjects.Add(appleCapHat);
        }
        
        if (blackSpecialHatPart1 == null)
        {
            Debug.LogError("PlayerHatManager: BlackSpecial hat part 1 GameObject is not assigned!");
        }
        else
        {
            allHatObjects.Add(blackSpecialHatPart1);
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
            string hatChoice = PlayerPrefs.GetString("Hat", "");
            
            // Convert the string to enum
            PlayerHat playerHat = PlayerHat.None; // Default is no hat
            if (HatNameToEnum.TryGetValue(hatChoice, out PlayerHat hat))
            {
                playerHat = hat;
            }
            else
            {
                Debug.LogWarning($"PlayerHatManager: Invalid hat choice '{hatChoice}', using no hat as default.");
            }
            
            // Set the networked value
            syncedPlayerHat.Value = playerHat;
        }

        // Everyone subscribes to the value change event
        syncedPlayerHat.OnValueChanged += OnPlayerHatChanged;
        
        // Apply the initial hat
        ApplyHat(syncedPlayerHat.Value);
    }

    public override void OnNetworkDespawn()
    {
        // Unsubscribe when despawning
        syncedPlayerHat.OnValueChanged -= OnPlayerHatChanged;
        base.OnNetworkDespawn();
    }

    private void OnPlayerHatChanged(PlayerHat previousValue, PlayerHat newValue)
    {
        ApplyHat(newValue);
    }

    private void DisableAllHats()
    {
        foreach (var hatObject in allHatObjects)
        {
            if (hatObject != null)
            {
                hatObject.SetActive(false);
            }
        }
    }

    private void ApplyHat(PlayerHat hat)
    {
        // First disable all hats
        DisableAllHats();
        
        // If no hat selected, just leave all disabled
        if (hat == PlayerHat.None) return;
        
        switch (hat)
        {
            case PlayerHat.Duck:
                if (duckHat != null)
                {
                    duckHat.SetActive(true);
                }
                break;
                
            case PlayerHat.AppleCap:
                if (appleCapHat != null)
                {
                    appleCapHat.SetActive(true);
                }
                break;
                
            case PlayerHat.BlackSpecial:
                if (blackSpecialHatPart1 != null)
                {
                    blackSpecialHatPart1.SetActive(true);
                }
                break;

            case PlayerHat.PreOrderHat:
                if (preOrderHat != null)
                {
                    preOrderHat.SetActive(true);
                }
                break;
                
            case PlayerHat.PartyHat:
                if(partyHat != null){
                    partyHat.SetActive(true);
                }
                break;
            case PlayerHat.PotHat:
                if(potHat != null){
                    potHat.SetActive(true);
                }
                break;
            case PlayerHat.SharkHat:
                if(sharkHat != null){
                    sharkHat.SetActive(true);
                }
                break;
            case PlayerHat.CrownHat:
                if(crownHat != null){
                    crownHat.SetActive(true);
                }
                break;
            case PlayerHat.DogEars:
                if(dogEars != null){
                    dogEars.SetActive(true);
                }
                break;
            case PlayerHat.GoldenPotHat:
                if(goldenPotHat != null){
                    goldenPotHat.SetActive(true);
                }
                break;
            case PlayerHat.Horns:
                if (horns != null)
                {
                    horns.SetActive(true);
                }
                break;
            case PlayerHat.AdminHorns:
                if (adminHorns != null)
                {
                    adminHorns.SetActive(true);
                }
                break;
            case PlayerHat.Glasses:
                if (glasses != null)
                {
                    glasses.SetActive(true);
                }
                break;
            case PlayerHat.SummerHat:
                if (summerHat != null)
                {
                    summerHat.SetActive(true);
                }
                break;

            default:
                Debug.LogError($"PlayerHatManager: Unhandled hat type: {hat}");
                break;
        }
    }
}