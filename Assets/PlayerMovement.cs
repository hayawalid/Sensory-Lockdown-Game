using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Movement settings
    public float moveSpeed = 5f;
    public float mouseSensitivity = 2f;
    
    // References
    private Rigidbody rb;
    public Transform headTransform;
    
    // For head rotation
    private float xRotation = 0f;
    
    // Ground check
    public float groundCheckDistance = 0.3f;
    public LayerMask groundLayer;
    private bool isGrounded;

    void Start()
    {
        // Get the Rigidbody component
        rb = GetComponent<Rigidbody>();
        
        // Lock and hide the cursor (comment these out if you want cursor visible while testing)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Handle mouse look (head movement)
        HandleMouseLook();
        
        // Check if player is on the ground
        CheckGround();
    }

    void FixedUpdate()
    {
        // Handle keyboard/joystick movement
        HandleMovement();
    }
    
    void CheckGround()
    {
        // Check if there's ground beneath the player
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance + 0.5f);
    }

    void HandleMovement()
    {
        // Get input from keyboard or joystick
        float moveX = Input.GetAxis("Horizontal"); // A/D or Left/Right arrows
        float moveZ = Input.GetAxis("Vertical");   // W/S or Up/Down arrows
        
        // Add a dead zone to ignore tiny inputs
        if (Mathf.Abs(moveX) < 0.1f) moveX = 0f;
        if (Mathf.Abs(moveZ) < 0.1f) moveZ = 0f;
        
        // Calculate movement direction relative to where player is facing
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        move = move.normalized * moveSpeed;
        
        // Apply movement while preserving gravity (Y velocity)
        // This allows horizontal movement but lets physics handle vertical
        Vector3 newVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);
        rb.linearVelocity = newVelocity;
    }

    void HandleMouseLook()
    {
        // Get mouse movement
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        
        // Rotate the body left/right (full 360° rotation)
        transform.Rotate(Vector3.up * mouseX);
        
        // Rotate the head up/down (limited to prevent looking too far up/down)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f); // Can look 80° up and 80° down
        
        if (headTransform != null)
        {
            headTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
    }
}
