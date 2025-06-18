using UnityEngine;
using Unity.Netcode;

public class LookInCameraLocally : MonoBehaviour
{
    // Reference to camera - will be set independently on each client
    private Camera localCamera;
    
    // Flag to track if this component has been properly initialized
    private bool isInitialized = false;
    
    private void Start()
    {
        FindActiveCamera();
    }
    
    private void OnEnable()
    {
        // Also initialize when enabled (in case objects are pooled or activated after Start)
        FindActiveCamera();
    }
    
    private void FindActiveCamera()
    {
        // Try to find an active camera in the scene
        if (!isInitialized)
        {
            // Find all cameras in the scene
            Camera[] allCameras = Camera.allCameras;
            
            // Look for an active camera
            foreach (Camera cam in allCameras)
            {
                if (cam.gameObject.activeInHierarchy)
                {
                    localCamera = cam;
                    isInitialized = true;
                    
                    // Force an initial orientation update
                    UpdateOrientation();
                    return;
                }
            }
            
            // If no active camera found, try again next frame
            if (!isInitialized)
            {
                Invoke("FindActiveCamera", 0.1f);
            }
        }
    }
    
    private void Update()
    {
        if (isInitialized)
        {
            // Check if our camera is still valid
            if (localCamera == null || !localCamera.gameObject.activeInHierarchy)
            {
                isInitialized = false;
                FindActiveCamera();
                return;
            }
            
            UpdateOrientation();
        }
        else
        {
            // Keep trying to initialize if not done yet
            FindActiveCamera();
        }
    }
    
    private void UpdateOrientation()
    {
        // Safety check
        if (localCamera == null)
        {
            isInitialized = false;
            return;
        }
        
        // Get the direction vector from the object to the camera (ignoring Y)
        Vector3 directionToCamera = localCamera.transform.position - transform.position;
        directionToCamera.y = 0; // Zero out the y component to keep rotation only on y-axis
        
        // Only rotate if we have a valid direction
        if (directionToCamera != Vector3.zero)
        {
            // Calculate the rotation needed to face the camera
            Quaternion targetRotation = Quaternion.LookRotation(directionToCamera);
            
            // Add 180 degrees to the Y rotation to fix mirrored text
            float yRotation = targetRotation.eulerAngles.y + 180f;
            
            // Apply the Y rotation with the 180-degree adjustment, keeping original X and Z rotations
            transform.rotation = Quaternion.Euler(
                transform.rotation.eulerAngles.x,
                yRotation,
                transform.rotation.eulerAngles.z
            );
        }
    }
}