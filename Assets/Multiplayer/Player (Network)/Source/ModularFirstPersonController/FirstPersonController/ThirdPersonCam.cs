using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCam : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;  // Camera orientation
    public Transform player;       // Player Transform
    public Transform playerObj;    // Player Object (for rotating)
    public Rigidbody rb;           // Rigidbody for movement
    public Transform cameraTransform;  // Camera Transform (Separate Camera)

    public float rotationSpeed = 10f;
    public float moveSpeed = 10f;  // Normal movement speed
    public float crouchHeight = 0.75f; // Height when crouching
    public float normalHeight = 2f;  // Normal player height
    public float crouchSpeed = 5f;  // Speed when crouching
    public float jumpForce = 5f;    // Jump force upwards

    private bool isCrouching = false;
    private bool isGrounded = true;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // Handle camera movement
        RotateCamera();

        // Handle player movement
        MovePlayer();

        // Handle jumping and crouching
        HandleJumping();
        HandleCrouching();
    }

    private void RotateCamera()
    {
        // Rotate the camera based on mouse input
        float horizontalInput = Input.GetAxis("Mouse X");
        float verticalInput = Input.GetAxis("Mouse Y");

        // Rotate the camera on the Y axis (left/right)
        orientation.Rotate(Vector3.up * horizontalInput * rotationSpeed * Time.deltaTime);

        // Rotate the player (on the X axis)
        float desiredRotationX = playerObj.rotation.eulerAngles.x - verticalInput * rotationSpeed * Time.deltaTime;
        desiredRotationX = Mathf.Clamp(desiredRotationX, -80f, 80f);  // Limit vertical rotation
        playerObj.rotation = Quaternion.Euler(desiredRotationX, playerObj.rotation.eulerAngles.y, 0);
    }

    private void MovePlayer()
    {
        // Get input for movement (W, A, S, D)
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Calculate movement direction relative to camera
        Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // If input is not zero, move the player in the direction
        if (inputDir != Vector3.zero)
        {
            // Apply movement based on input direction
            Vector3 moveDir = inputDir.normalized;

            // When crouching, move slower
            if (isCrouching)
            {
                rb.MovePosition(rb.position + moveDir * crouchSpeed * Time.deltaTime);
            }
            else
            {
                rb.MovePosition(rb.position + moveDir * moveSpeed * Time.deltaTime);
            }
        }
    }

    private void HandleJumping()
    {
        // Check if player is grounded and pressing jump button (Spacebar)
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;  // Make sure player is not jumping again until they land
        }
    }

    private void HandleCrouching()
    {
        // Check if player is pressing the crouch button (Left Control)
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (isCrouching)
            {
                // Stand up (reset height and speed)
                isCrouching = false;
                player.localScale = new Vector3(1, 1, 1);  // Reset player scale
                moveSpeed = 10f;  // Reset normal speed
            }
            else
            {
                // Crouch (lower height and reduce speed)
                isCrouching = true;
                player.localScale = new Vector3(1, crouchHeight, 1);  // Reduce player height
                moveSpeed = crouchSpeed;  // Halve the movement speed when crouching
            }
        }
    }

    // Detect when player is grounded (simple check for ground collision)
    private void OnCollisionEnter(Collision collision)
    {
        isGrounded = true;  // Reset isGrounded when player hits the ground
    }
}
