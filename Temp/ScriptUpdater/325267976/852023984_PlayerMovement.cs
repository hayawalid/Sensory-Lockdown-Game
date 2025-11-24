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

    void Start()
    {
        // Get the Rigidbody component
        rb = GetComponent<Rigidbody>();
        
        // Lock and hide the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Handle mouse look (head movement)
        HandleMouseLook();
    }

    void FixedUpdate()
    {
        // Handle keyboard/joystick movement
        HandleMovement();
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
    
    // Apply movement while keeping the current Y velocity (gravity)
    rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);
}

    void HandleMouseLook()
    {
        // Get mouse movement
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        
        // Rotate the body left/right
        transform.Rotate(Vector3.up * mouseX);
        
        // Rotate the head up/down
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Limit looking up/down
        
        if (headTransform != null)
        {
            headTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
    }
}
