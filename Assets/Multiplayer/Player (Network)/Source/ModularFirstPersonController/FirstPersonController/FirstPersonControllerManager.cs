using UnityEngine;

public class FirstPersonControllerManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private FirstPersonController firstPersonController;
    [SerializeField] private AnimationController animationController;
    [SerializeField] private GameObject hintText;

    [Header("Settings")]
    [SerializeField] private float doubleClickTime = 0.3f;

    private bool isControlLocked = false;
    private float lastClickTime = 0f;
    private bool isFirstClick = true;

    private void Start()
    {
        if (hintText != null)
            hintText.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Slash))
        {
            ToggleControl();
        }

        if (isControlLocked && Input.GetMouseButtonDown(0))
        {
            float timeSinceLastClick = Time.time - lastClickTime;

            if (timeSinceLastClick <= doubleClickTime)
            {
                ToggleControl();
                isFirstClick = true;
            }
            else
            {
                isFirstClick = false;
            }

            lastClickTime = Time.time;
        }
    }

    private void ToggleControl()
    {
        isControlLocked = !isControlLocked;

        Cursor.lockState = isControlLocked ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isControlLocked;

        if (firstPersonController != null)
        {
            firstPersonController.playerCanMove = !isControlLocked;
            firstPersonController.enableJump = !isControlLocked;
            firstPersonController.enableCrouch = !isControlLocked;  // Added Crouch control toggle
        }

        if (animationController != null)
        {
            animationController.enabled = !isControlLocked;
        }

        if (hintText != null)
        {
            hintText.SetActive(isControlLocked);
        }
    }
}
