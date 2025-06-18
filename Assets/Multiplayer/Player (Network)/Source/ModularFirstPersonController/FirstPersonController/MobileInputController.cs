using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MobileInputController : MonoBehaviour
{
    [Header("References")]
    public FirstPersonController playerController;
    public Camera playerCamera;

    [Header("Mobile Controls")]
    public float touchSensitivity = 0.5f;
    public float joystickRadius = 100f;
    public float sprintThreshold = 0.8f;

    [Header("UI References")]
    public Image moveJoystickBackground;
    public Image moveJoystickHandle;
    public Image lookJoystickBackground;
    public Image lookJoystickHandle;
    public Button jumpButton;
    public Button sprintButton;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector2 moveJoystickOrigin;
    private Vector2 lookJoystickOrigin;
    private bool isSprinting;
    private bool isMobilePlatform;

    private void Start()
    {
        // Check if we're on mobile platform
        isMobilePlatform = PlayerPrefs.GetString("Platform", "PC") == "Mobile";
        
        if (!isMobilePlatform)
        {
            // Disable all UI elements if not on mobile
            if (moveJoystickBackground != null) moveJoystickBackground.gameObject.SetActive(false);
            if (moveJoystickHandle != null) moveJoystickHandle.gameObject.SetActive(false);
            if (lookJoystickBackground != null) lookJoystickBackground.gameObject.SetActive(false);
            if (lookJoystickHandle != null) lookJoystickHandle.gameObject.SetActive(false);
            if (jumpButton != null) jumpButton.gameObject.SetActive(false);
            if (sprintButton != null) sprintButton.gameObject.SetActive(false);
            return;
        }

        // Initialize UI elements
        if (moveJoystickBackground != null && moveJoystickHandle != null)
        {
            moveJoystickOrigin = moveJoystickBackground.rectTransform.anchoredPosition;
        }

        if (lookJoystickBackground != null && lookJoystickHandle != null)
        {
            lookJoystickOrigin = lookJoystickBackground.rectTransform.anchoredPosition;
        }

        // Setup button listeners
        if (jumpButton != null)
        {
            jumpButton.onClick.AddListener(OnJumpButtonPressed);
        }

        if (sprintButton != null)
        {
            sprintButton.onClick.AddListener(OnSprintButtonPressed);
        }
    }

    private void Update()
    {
        if (!isMobilePlatform) return;
        
        HandleTouchInput();
        UpdateJoystickPositions();
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            foreach (Touch touch in Input.touches)
            {
                // Skip if touch is over UI elements
                if (IsPointerOverUI(touch.position))
                    continue;

                // Handle movement joystick
                if (IsTouchOverJoystick(touch.position, moveJoystickBackground))
                {
                    HandleMoveJoystick(touch);
                }
                // Handle look joystick
                else if (IsTouchOverJoystick(touch.position, lookJoystickBackground))
                {
                    HandleLookJoystick(touch);
                }
            }
        }
    }

    private void HandleMoveJoystick(Touch touch)
    {
        Vector2 touchPos = touch.position;
        Vector2 direction = touchPos - moveJoystickOrigin;
        float distance = direction.magnitude;

        if (distance > joystickRadius)
        {
            direction = direction.normalized * joystickRadius;
        }

        moveInput = direction / joystickRadius;
        moveJoystickHandle.rectTransform.anchoredPosition = moveJoystickOrigin + direction;
    }

    private void HandleLookJoystick(Touch touch)
    {
        Vector2 touchPos = touch.position;
        Vector2 direction = touchPos - lookJoystickOrigin;
        float distance = direction.magnitude;

        if (distance > joystickRadius)
        {
            direction = direction.normalized * joystickRadius;
        }

        lookInput = direction / joystickRadius;
        lookJoystickHandle.rectTransform.anchoredPosition = lookJoystickOrigin + direction;
    }

    private void UpdateJoystickPositions()
    {
        // Update movement input in FirstPersonController
        if (playerController != null)
        {
            // Apply sprint if sprint button is pressed
            if (isSprinting && moveInput.magnitude > sprintThreshold)
            {
               // playerController.sprintKey = KeyCode.LeftShift;
            }
            else
            {
               // playerController.sprintKey = KeyCode.None;
            }

            // Apply movement
            if (moveInput.magnitude > 0)
            {
                // Calculate movement direction
                Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
                moveDirection = playerCamera.transform.TransformDirection(moveDirection);
                moveDirection.y = 0; // Keep movement horizontal
                moveDirection.Normalize();

                // Apply movement to the player
                playerController.transform.position += moveDirection * playerController.walkSpeed * Time.deltaTime;
            }
        }

        // Update camera rotation
        if (playerCamera != null && lookInput.magnitude > 0)
        {
            float mouseX = lookInput.x * touchSensitivity;
            float mouseY = lookInput.y * touchSensitivity;

            // Apply rotation to camera
            playerCamera.transform.RotateAround(playerCamera.transform.position, Vector3.up, mouseX);
            playerCamera.transform.RotateAround(playerCamera.transform.position, playerCamera.transform.right, -mouseY);
        }
    }

    private void OnJumpButtonPressed()
    {
        //if (playerController != null && playerController.isGrounded)
      //  {
       //     playerController.Jump();
      //  }
    }

    private void OnSprintButtonPressed()
    {
        isSprinting = !isSprinting;
    }

    private bool IsTouchOverJoystick(Vector2 touchPos, Image joystickBackground)
    {
        if (joystickBackground == null) return false;

        RectTransform rectTransform = joystickBackground.rectTransform;
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform.parent as RectTransform,
            touchPos,
            null,
            out localPoint
        );

        return rectTransform.rect.Contains(localPoint);
    }

    private bool IsPointerOverUI(Vector2 position)
    {
        if (EventSystem.current == null) return false;
        return EventSystem.current.IsPointerOverGameObject();
    }
} 