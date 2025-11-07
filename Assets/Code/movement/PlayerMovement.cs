using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(IInputProvider))]
public class PlayerMovement : MonoBehaviour
{
    private const float walkSpeed = 5f;
    private const float runSpeed = 8f;
    private const float mouseSensitivity = 2f;
    
    // Bunnyhop mechanics
    private const float jumpForce = 7f;
    private const float maxSpeed = 12f; // Speed cap like CS 1.6
    private const float airAcceleration = 10f; // How fast you accelerate in air
    private const float groundFriction = 6f; // Ground friction
    private const float airFriction = 0.1f; // Air friction (much lower)
    private const float groundAcceleration = 15f; // Ground acceleration

    private Rigidbody rb;
    private IInputProvider inputProvider;
    private Camera playerCamera;

    private float currentSpeed;
    private bool isGrounded = true;
    private bool wasGrounded = true;
    private Vector3 velocity;
    private Vector3 wishDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        inputProvider = GetComponent<IInputProvider>();
        playerCamera = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        HandleLook();
        CheckGrounded();
        HandleJump();
    }
    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleLook()
    {
        Vector2 lookInput = inputProvider.GetLookInput();
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);
        float camRotationX = playerCamera.transform.localRotation.eulerAngles.x - mouseY;

        playerCamera.transform.localRotation = Quaternion.Euler(camRotationX, 0f, 0f);
    }

    private void HandleMovement()
    {
        Vector2 moveInput = inputProvider.GetMovementInput();
        wishDirection = transform.right * moveInput.x + transform.forward * moveInput.y;
        wishDirection = new Vector3(wishDirection.x, 0, wishDirection.z).normalized;

        bool isRunning = inputProvider.IsRunPressed() && moveInput.magnitude > 0;
        float baseSpeed = isRunning ? runSpeed : walkSpeed;
        currentSpeed = inputProvider.IsCrouchPressed() && !isRunning ? baseSpeed * 0.5f : baseSpeed;

        velocity = rb.linearVelocity;

        if (isGrounded)
        {
            ApplyGroundMovement();
        }
        else
        {
            ApplyAirMovement();
        }

        // Apply speed cap
        Vector3 horizontalVelocity = new Vector3(velocity.x, 0, velocity.z);
        if (horizontalVelocity.magnitude > maxSpeed)
        {
            horizontalVelocity = horizontalVelocity.normalized * maxSpeed;
            velocity = new Vector3(horizontalVelocity.x, velocity.y, horizontalVelocity.z);
        }

        rb.linearVelocity = velocity;
    }

    private void ApplyGroundMovement()
    {
        // Ground movement with friction
        Vector3 horizontalVelocity = new Vector3(velocity.x, 0, velocity.z);
        
        // Apply friction
        float friction = groundFriction * Time.fixedDeltaTime;
        horizontalVelocity = Vector3.Lerp(horizontalVelocity, Vector3.zero, friction);
        
        // Apply acceleration
        if (wishDirection.magnitude > 0)
        {
            float acceleration = groundAcceleration * Time.fixedDeltaTime;
            horizontalVelocity = Vector3.Lerp(horizontalVelocity, wishDirection * currentSpeed, acceleration);
        }
        
        velocity = new Vector3(horizontalVelocity.x, velocity.y, horizontalVelocity.z);
    }

    private void ApplyAirMovement()
    {
        // Air strafing - bunnyhop mechanics
        Vector3 horizontalVelocity = new Vector3(velocity.x, 0, velocity.z);
        
        // Apply minimal air friction
        float friction = airFriction * Time.fixedDeltaTime;
        horizontalVelocity = Vector3.Lerp(horizontalVelocity, horizontalVelocity, friction);
        
        // Air strafing - allows you to change direction while maintaining speed
        if (wishDirection.magnitude > 0)
        {
            Vector3 wishVelocity = wishDirection * currentSpeed;
            Vector3 velocityDiff = wishVelocity - horizontalVelocity;
            
            // Limit acceleration to prevent instant direction changes
            float acceleration = airAcceleration * Time.fixedDeltaTime;
            Vector3 accelerationVector = velocityDiff.normalized * Mathf.Min(velocityDiff.magnitude, acceleration);
            
            horizontalVelocity += accelerationVector;
        }
        
        velocity = new Vector3(horizontalVelocity.x, velocity.y, horizontalVelocity.z);
    }

    private void HandleJump()
    {
        // Check if jump button is pressed and we're grounded
        // Using Unity's Input system as fallback
        bool jumpPressed = Input.GetKeyDown(KeyCode.Space) || (inputProvider != null && inputProvider.IsJumpPressed());
        
        if (jumpPressed && isGrounded)
        {
            // Jump
            velocity.y = jumpForce;
            rb.linearVelocity = velocity;
        }
        
        wasGrounded = isGrounded;
    }

    private void CheckGrounded()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f, LayerMask.GetMask("Ground"));
    }

    public bool IsGrounded() => isGrounded;
    
    public float GetCurrentSpeed()
    {
        // Return horizontal speed in km/h (multiply by 3.6 to convert from m/s to km/h)
        Vector3 horizontalVelocity = new Vector3(velocity.x, 0, velocity.z);
        return horizontalVelocity.magnitude * 3.6f;
    }
}
