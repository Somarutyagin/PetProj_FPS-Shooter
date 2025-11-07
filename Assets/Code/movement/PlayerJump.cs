using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(IInputProvider))]
[RequireComponent(typeof(PlayerMovement))]
public class PlayerJump : MonoBehaviour
{
    private const float jumpForce = 5f;
    private const float jumpCooldown = 0.1f;
    private const float groundCheckDistance = 1.2f;
    
    private Rigidbody rb;
    private IInputProvider inputProvider;
    private PlayerMovement movement;
    
    private bool jumpPressed = false;
    private float lastJumpTime = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        inputProvider = GetComponent<IInputProvider>();
        movement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        // Check for jump input in Update to catch all input frames
        if (inputProvider.IsJumpPressed())
        {
            jumpPressed = true;
        }
    }

    private void FixedUpdate()
    {
        // Handle jump in FixedUpdate for consistent physics
        if (jumpPressed && CanJump())
        {
            PerformJump();
            jumpPressed = false;
            lastJumpTime = Time.time;
        }
    }
    
    private bool CanJump()
    {
        // Check if enough time has passed since last jump
        if (Time.time - lastJumpTime < jumpCooldown)
            return false;
            
        // Check if grounded with more lenient ground check
        return IsGrounded();
    }
    
    private bool IsGrounded()
    {
        // More reliable ground check with multiple raycasts
        Vector3 origin = transform.position;
        float checkDistance = groundCheckDistance;
        
        // Check center
        if (Physics.Raycast(origin, Vector3.down, checkDistance))
            return true;
            
        // Check corners for better edge detection
        float offset = 0.3f;
        Vector3[] corners = {
            origin + new Vector3(offset, 0, offset),
            origin + new Vector3(-offset, 0, offset),
            origin + new Vector3(offset, 0, -offset),
            origin + new Vector3(-offset, 0, -offset)
        };
        
        foreach (Vector3 corner in corners)
        {
            if (Physics.Raycast(corner, Vector3.down, checkDistance))
                return true;
        }
        
        return false;
    }
    
    private void PerformJump()
    {
        // Reset vertical velocity before jumping for consistent jump height
        Vector3 velocity = rb.linearVelocity;
        velocity.y = 0f;
        rb.linearVelocity = velocity;
        
        // Apply jump force
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
}
