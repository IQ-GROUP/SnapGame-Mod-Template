using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections; // Added for coroutines

public class LocalBuildingMode : MonoBehaviour
{
    public GameObject[] showcasePrefabs;
    public GameObject[] buildingPrefabs;
    public TextMeshProUGUI buildingModeText;
    public TextMeshProUGUI kinematicModeText; // Text element for kinematic mode status
    public TextMeshProUGUI destroyModeText; // New text element for destroy mode status
    public GameObject advancedTipsPanel;
    public GameObject destroyZone; // The zone GameObject for destroy mode
    
    private GameObject currentObject;
    private int currentIndex = 0;
    private List<GameObject> showcaseObjects = new List<GameObject>();
    private List<GameObject> builtObjects = new List<GameObject>();
    private Vector3 moveSpeed = new Vector3(0.03f, 0.03f, 0.03f);
    private Vector3 rotationSpeed = new Vector3(1.67f, 1.67f, 1.67f);
    private float scaleSpeed = 0.1f; // Speed for scaling objects
    
    private NetworkBuildingManager networkBuildingManager;
    [HideInInspector] public bool isBuildingModeActive = false;
    private bool canPlace = true; // Variable to track if placement is allowed
    private float placementCooldown = 0.5f; // Cooldown in seconds
    private bool isKinematicMode = false; // Controls whether placed objects will be kinematic
    private bool isDestroyModeActive = false; // New variable for destroy mode
    private List<GameObject> objectsInDestroyZone = new List<GameObject>(); // Track objects in destroy zone

    void Awake()
    {
        networkBuildingManager = FindFirstObjectByType<NetworkBuildingManager>();
        if (networkBuildingManager == null)
            Debug.LogError("NetworkBuildingManager not found!");
    }
    
    void Start()
    {
        // Initialize UI texts
        UpdateBuildingModeText();
        UpdateKinematicModeText();
        UpdateDestroyModeText();
        
        // Initialize destroy zone
        if (destroyZone != null)
        {
            destroyZone.SetActive(false);
            
            // Add a trigger collider to the destroy zone if it doesn't have one
            if (destroyZone.GetComponent<Collider>() == null)
            {
                BoxCollider boxCollider = destroyZone.AddComponent<BoxCollider>();
                boxCollider.isTrigger = true;
                boxCollider.size = new Vector3(5f, 5f, 5f); // Default size, adjust as needed
            }
            
            // Add the DestroyZoneTrigger component to handle collisions
            if (destroyZone.GetComponent<DestroyZoneTrigger>() == null)
            {
                DestroyZoneTrigger trigger = destroyZone.AddComponent<DestroyZoneTrigger>();
                trigger.localBuildingMode = this;
            }
        }
        else
        {
            Debug.LogWarning("Destroy Zone not assigned in LocalBuildingMode!");
        }
    }
    
    void Update()
    {
        float floatMoveSpeed = PlayerPrefs.GetFloat("BuildingMoveSpeed");
        if(floatMoveSpeed > 0)
        moveSpeed = new Vector3(floatMoveSpeed, floatMoveSpeed, floatMoveSpeed);

        float floatRotationSpeed = PlayerPrefs.GetFloat("BuildingRotationSpeed");
        if(floatRotationSpeed > 0)
        rotationSpeed = new Vector3(floatRotationSpeed, floatRotationSpeed, floatRotationSpeed);

        // Toggle building mode with C key
        if (Input.GetKeyDown(KeyCode.C))
        {
            // Cannot activate building mode while destroy mode is active
            if (isDestroyModeActive && !isBuildingModeActive)
                return;
                
            isBuildingModeActive = !isBuildingModeActive;
            Debug.Log("Building Mode: " + (isBuildingModeActive ? "Activated" : "Deactivated"));
            UpdateBuildingModeText();
            ToggleAdvancedTips();

            if (!isBuildingModeActive && currentObject != null)
            {
                Destroy(currentObject);
                currentObject = null;
            }
            
            // If building mode is deactivated and destroy mode was active, reactivate destroy zone
            if (!isBuildingModeActive && isDestroyModeActive)
            {
                if (destroyZone != null)
                    destroyZone.SetActive(true);
            }
        }

        // Toggle kinematic mode with X key
        if (Input.GetKeyDown(KeyCode.X) && isBuildingModeActive)
        {
            isKinematicMode = !isKinematicMode;
            Debug.Log("Kinematic Mode: " + (isKinematicMode ? "On" : "Off"));
            UpdateKinematicModeText();
        }
        
        // Toggle destroy mode with Z key
        if (Input.GetKeyDown(KeyCode.Z))
        {
            isDestroyModeActive = !isDestroyModeActive;
            Debug.Log("Destroy Mode: " + (isDestroyModeActive ? "Activated" : "Deactivated"));
            UpdateDestroyModeText();
            
            // If destroy mode is activated, ensure building mode is off
            if (isDestroyModeActive)
            {
                if (isBuildingModeActive)
                {
                    isBuildingModeActive = false;
                    UpdateBuildingModeText();
                    
                    if (currentObject != null)
                    {
                        Destroy(currentObject);
                        currentObject = null;
                    }
                }
                
                // Hide advanced tips when in destroy mode
                if (advancedTipsPanel != null)
                    advancedTipsPanel.SetActive(false);
                
                // Show destroy zone
                if (destroyZone != null)
                    destroyZone.SetActive(true);
            }
            else
            {
                // Hide destroy zone when exiting destroy mode
                if (destroyZone != null)
                    destroyZone.SetActive(false);
                    
                // Clear the list of objects in destroy zone
                objectsInDestroyZone.Clear();
            }
        }

        if (!isBuildingModeActive && !isDestroyModeActive) return;
        
        if (isBuildingModeActive)
        {
            HandleBuildingSelection();
            HandleObjectMovement();
            HandleObjectRotation();
            HandleObjectScaling();
            ConfirmPlacement();
        }
        
        if (isDestroyModeActive)
        {
            HandleDestruction();
        }
    }

    void HandleBuildingSelection()
    {
        if (showcasePrefabs.Length == 0) return;

        // Scroll up
        if (Input.GetAxis("Mouse ScrollWheel") > 0f || Input.GetKeyDown(KeyCode.E))
        {
            currentIndex = (currentIndex + 1) % showcasePrefabs.Length;
            SelectCurrentObject();
        }
        // Scroll down
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f || Input.GetKeyDown(KeyCode.Q))
        {
            currentIndex = (currentIndex - 1 + showcasePrefabs.Length) % showcasePrefabs.Length;
            SelectCurrentObject();
        }
    }

    void HandleObjectMovement()
    {
        if (currentObject == null) return;

        Transform objTransform = currentObject.transform;

        if (Input.GetKey(KeyCode.UpArrow))
            objTransform.localPosition += objTransform.right * moveSpeed.x;
        if (Input.GetKey(KeyCode.DownArrow))
            objTransform.localPosition += -objTransform.right * moveSpeed.x;
        if (Input.GetKey(KeyCode.LeftArrow))
            objTransform.localPosition += objTransform.forward * moveSpeed.z;
        if (Input.GetKey(KeyCode.RightArrow))
            objTransform.localPosition += -objTransform.forward * moveSpeed.z;
        if (Input.GetKey(KeyCode.PageUp) || Input.GetKey(KeyCode.Y))
            objTransform.localPosition += objTransform.up * moveSpeed.y;
        if (Input.GetKey(KeyCode.PageDown) || Input.GetKey(KeyCode.H))
            objTransform.localPosition += -objTransform.up * moveSpeed.y;
    }

    void HandleObjectRotation()
    {
        if (currentObject == null) return;
        
        if (Input.GetKey(KeyCode.U))
            currentObject.transform.Rotate(Vector3.right * rotationSpeed.x, Space.Self);
        if (Input.GetKey(KeyCode.J))
            currentObject.transform.Rotate(Vector3.left * rotationSpeed.x, Space.Self);
        if (Input.GetKey(KeyCode.I))
            currentObject.transform.Rotate(Vector3.up * rotationSpeed.y, Space.Self);
        if (Input.GetKey(KeyCode.K))
            currentObject.transform.Rotate(Vector3.down * rotationSpeed.y, Space.Self);
        if (Input.GetKey(KeyCode.O))
            currentObject.transform.Rotate(Vector3.forward * rotationSpeed.z, Space.Self);
        if (Input.GetKey(KeyCode.L))
            currentObject.transform.Rotate(Vector3.back * rotationSpeed.z, Space.Self);
    }
    
    void HandleObjectScaling()
    {
        if (currentObject == null) return;
        
        if (Input.GetKey(KeyCode.N)) // Scale up
            currentObject.transform.localScale += new Vector3(scaleSpeed, scaleSpeed, scaleSpeed) * Time.deltaTime;
        if (Input.GetKey(KeyCode.M)) // Scale down
            currentObject.transform.localScale -= new Vector3(scaleSpeed, scaleSpeed, scaleSpeed) * Time.deltaTime;
    }

    void ConfirmPlacement()
    {
        if (currentObject != null && Input.GetMouseButtonDown(0) && canPlace)
        {
            canPlace = false; // Disable placement during cooldown
            
            int index = -1;
            for (int i = 0; i < showcasePrefabs.Length; i++)
            {
                if (currentObject.name.Contains(showcasePrefabs[i].name)) // Compare names to find the correct index
                {
                    index = i;
                    break;
                }
            }

            if (index >= 0 && index < buildingPrefabs.Length)
            {
                Vector3 position = currentObject.transform.position;
                Quaternion rotation = currentObject.transform.rotation;
                Vector3 scale = currentObject.transform.localScale;
                
                // Instantiate the building object
                InstantiateBuildingObject(index, position, rotation, scale);

                // Don't destroy the current object - let it remain for continued placement
                // The line below is removed: Destroy(currentObject);
                // The line below is removed: currentObject = null;
            }
            else
            {
                Debug.LogError("Error: Invalid index when placing building object.");
            }
            
            // Start cooldown coroutine
            StartCoroutine(PlacementCooldown());
        }
    }
    
    void HandleDestruction()
    {
        // If left mouse button is clicked and there are objects in the destroy zone
        if (Input.GetMouseButtonDown(0) && objectsInDestroyZone.Count > 0)
        {
            foreach (GameObject obj in new List<GameObject>(objectsInDestroyZone))
            {
                if (obj != null)
                {
                    // Check if the object has a NetworkObject component
                    Unity.Netcode.NetworkObject netObj = obj.GetComponent<Unity.Netcode.NetworkObject>();
                    if (netObj != null && netObj.IsSpawned && netObj.CompareTag("BuildingObject"))
                    {
                        // Use the NetworkBuildingManager to destroy the object
                        networkBuildingManager.DestroyBuildingObject(netObj.NetworkObjectId);
                    }
                }
            }
            
            // Clear the list after attempting to destroy all objects
            objectsInDestroyZone.Clear();
        }
    }
    
    // Coroutine to handle placement cooldown
    private IEnumerator PlacementCooldown()
    {
        yield return new WaitForSeconds(placementCooldown);
        canPlace = true; // Re-enable placement after cooldown
    }

    void InstantiateBuildingObject(int prefabIndex, Vector3 position, Quaternion rotation, Vector3 scale)
    {
        Debug.Log($"Instantiating prefab index: {prefabIndex}, Kinematic: {isKinematicMode}");
        networkBuildingManager.InstantiateBuildingObject(prefabIndex, position, rotation, scale, isKinematicMode);
    }

    void UpdateBuildingModeText()
    {
        if (buildingModeText != null)
        {
            buildingModeText.text = $"{(isBuildingModeActive ? "On" : "Off")}";
        }
    }
    
    void UpdateKinematicModeText()
    {
        if (kinematicModeText != null)
        {
            kinematicModeText.text = $"{(isKinematicMode ? "Off" : "On")}";
        }
    }
    
    void UpdateDestroyModeText()
    {
        if (destroyModeText != null)
        {
            destroyModeText.text = $"{(isDestroyModeActive ? "On" : "Off")}";
        }
    }

    void ToggleAdvancedTips()
    {
        if (advancedTipsPanel != null)
        {
            advancedTipsPanel.SetActive(isBuildingModeActive && !isDestroyModeActive);
        }
    }
    
    // Methods to manage objects in destroy zone
    public void AddObjectToDestroyZone(GameObject obj)
    {
        if (!objectsInDestroyZone.Contains(obj))
        {
            objectsInDestroyZone.Add(obj);
            Debug.Log($"Object added to destroy zone: {obj.name}");
        }
    }
    
    public void RemoveObjectFromDestroyZone(GameObject obj)
    {
        if (objectsInDestroyZone.Contains(obj))
        {
            objectsInDestroyZone.Remove(obj);
            Debug.Log($"Object removed from destroy zone: {obj.name}");
        }
    }
    void SelectCurrentObject()
    {
        if (currentObject != null)
            Destroy(currentObject);

        currentObject = Instantiate(showcasePrefabs[currentIndex], transform.position, transform.rotation);
        Debug.Log("Current object selected: " + currentObject.name);
    }
}

// New component to detect objects entering the destroy zone
public class DestroyZoneTrigger : MonoBehaviour
{
    public LocalBuildingMode localBuildingMode;
    
    private void OnTriggerEnter(Collider other)
    {
        // Ignore non-networked objects
        if (other.GetComponent<Unity.Netcode.NetworkObject>() != null)
        {
            localBuildingMode.AddObjectToDestroyZone(other.gameObject);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        localBuildingMode.RemoveObjectFromDestroyZone(other.gameObject);
    }
}