using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    private Rigidbody rb;
    public GameObject playerCamera;

    #region Movement Variables

    public bool playerCanMove = true;
    public float walkSpeed = 5f;
    public float jumpPower = 5f;
    public float crouchHeight = 0.5f;
    private Vector3 originalScale;

    public bool isCrouched { get; private set; }
    public bool isFalling { get; private set; }
    public bool isJumpingThisFrame { get; private set; }

    public Vector3 CurrentVelocity { get { return rb.linearVelocity; } }

    public float mouseSensitivity = 2f;
    public float maxLookAngle = 80f;

    private float verticalRotation = 0f; // For controlling pitch (up/down)
    private float yaw = 0f; // For controlling yaw (left/right)

    private Quaternion targetRotation;

    #endregion

    #region Jump and Crouch

    public bool enableJump = true;
    public bool enableCrouch = true;

    #endregion

    // Platform tracking
    private Rigidbody standingOnRigidbody;
    private Vector3 inputDirection;

    private bool isGrounded;

    void CheckGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.1f))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        originalScale = transform.localScale;

        targetRotation = rb.rotation;  // Initialize targetRotation to current rotation
    }

    void Update()
    {
        CheckGround(); // <-- Make sure this is called before movement/jumping

        if (playerCanMove)
        {
            HandleMovement();
            HandleMouseLook();
            HandleJumping();
            HandleCrouching();
        }

        isFalling = rb.linearVelocity.y < -0.1f;
    }

    void FixedUpdate()
    {
        if (playerCanMove)
        {
            // Smoothly rotate towards target rotation using Rigidbody.MoveRotation
            if (targetRotation != Quaternion.identity)
            {
                Quaternion newRotation = Quaternion.Slerp(rb.rotation, targetRotation, Time.fixedDeltaTime * 15f).normalized;
                rb.MoveRotation(newRotation);
            }
        }
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 cameraForward = playerCamera.transform.forward;
        Vector3 cameraRight = playerCamera.transform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;
        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 rawDirection = (cameraRight * moveX + cameraForward * moveZ).normalized;

        float moveSpeed = isCrouched ? walkSpeed * 0.5f : walkSpeed;

        Vector3 movement = rawDirection;
        if (isGrounded)
        {
            movement = movement.normalized;
        }

        Vector3 velocity = movement * moveSpeed;
        velocity.y = rb.linearVelocity.y; // Preserve Y velocity (for jumping/falling)

        if (standingOnRigidbody != null)
        {
            velocity += new Vector3(standingOnRigidbody.linearVelocity.x, 0f, standingOnRigidbody.linearVelocity.z);
        }

        rb.linearVelocity = velocity;

        if (movement.magnitude > 0.1f)
        {
            targetRotation = Quaternion.LookRotation(movement).normalized;
        }
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        yaw += mouseX;
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -maxLookAngle, maxLookAngle);

        playerCamera.transform.rotation = Quaternion.Euler(verticalRotation, yaw, 0f);
    }

    void HandleJumping()
    {
        if (enableJump && Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            isJumpingThisFrame = true;
        }
        else
        {
            isJumpingThisFrame = false;
        }
    }

    void HandleCrouching()
    {
        if (!enableCrouch) return;

        if (Input.GetKey(KeyCode.LeftControl))
        {
            transform.localScale = new Vector3(1f, crouchHeight, 1f);
            isCrouched = true;
        }
        else
        {
            transform.localScale = originalScale;
            isCrouched = false;
        }
    }
}
