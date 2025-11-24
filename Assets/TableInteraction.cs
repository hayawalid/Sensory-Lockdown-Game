using UnityEngine;
using TMPro;

public class TableInteraction : MonoBehaviour
{
    [Header("UI References")]
    public GameObject notificationBox; // The cute notification box
    public TextMeshProUGUI notificationText; // The text inside the box
    
    [Header("Puzzle Scene")]
    public Transform puzzleCamera; // The camera from your player's head
    public Transform puzzleViewPosition; // Where camera should move to view the puzzle
    public float transitionSpeed = 5f;
    
    [Header("Settings")]
    public KeyCode interactKey = KeyCode.E; // Key to press (E for PC)
    public KeyCode exitKey = KeyCode.Escape; // Key to exit puzzle view
    
    private bool playerNearby = false;
    private bool isPuzzleView = false;
    private bool isTransitioning = false;
    
    // Store original camera info
    private Transform originalCameraParent;
    private Vector3 originalLocalPosition;
    private Quaternion originalLocalRotation;
    private PlayerMovement playerMovement;

    void Start()
    {
        // Hide the notification at start
        if (notificationBox != null)
            notificationBox.SetActive(false);
            
        // Find player movement script
        playerMovement = FindObjectOfType<PlayerMovement>();
        
        // Store original camera parent and local transform
        if (puzzleCamera != null)
        {
            originalCameraParent = puzzleCamera.parent;
            originalLocalPosition = puzzleCamera.localPosition;
            originalLocalRotation = puzzleCamera.localRotation;
        }
    }

    void Update()
    {
        // Check for interaction input when near table
        if (playerNearby && !isPuzzleView && !isTransitioning)
        {
            // PC/Laptop - Keyboard
            if (Input.GetKeyDown(interactKey))
            {
                EnterPuzzleView();
            }
            
            // Mobile - Touch (tap anywhere on screen)
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                EnterPuzzleView();
            }
        }
        
        // Exit puzzle view - ONLY when in puzzle mode
        if (isPuzzleView && !isTransitioning)
        {
            // Press ESC or E to exit
            if (Input.GetKeyDown(exitKey) || Input.GetKeyDown(interactKey))
            {
                ExitPuzzleView();
            }
            
            // Mobile - tap to exit
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                ExitPuzzleView();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the player entered the trigger zone
        if (other.CompareTag("Player") || other.GetComponent<PlayerMovement>() != null)
        {
            playerNearby = true;
            
            // Show the notification
            if (notificationBox != null)
            {
                notificationBox.SetActive(true);
                if (notificationText != null)
                    notificationText.text = "Press E to examine puzzle";
            }
                
            Debug.Log("Player can interact with table!");
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Check if the player left the trigger zone
        if (other.CompareTag("Player") || other.GetComponent<PlayerMovement>() != null)
        {
            playerNearby = false;
            
            // Hide the notification (only if not in puzzle view)
            if (!isPuzzleView && notificationBox != null)
                notificationBox.SetActive(false);
                
            Debug.Log("Player left table area");
        }
    }

    void EnterPuzzleView()
    {
        isPuzzleView = true;
        isTransitioning = true;
        Debug.Log("Entering puzzle view!");
        
        // DISABLE ALL PLAYER CONTROLS
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }
        
        // Unlock and show cursor for puzzle interaction
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        // Detach camera from player head
        if (puzzleCamera != null)
        {
            puzzleCamera.SetParent(null);
        }
        
        // Start smooth transition to puzzle view
        StartCoroutine(TransitionCameraToPuzzle());
    }

    void ExitPuzzleView()
    {
        isPuzzleView = false;
        isTransitioning = true;
        Debug.Log("Exiting puzzle view!");
        
        // Start smooth transition back to player
        StartCoroutine(TransitionCameraToPlayer());
    }
    
    System.Collections.IEnumerator TransitionCameraToPuzzle()
    {
        float elapsed = 0f;
        Vector3 startPos = puzzleCamera.position;
        Quaternion startRot = puzzleCamera.rotation;
        
        Vector3 targetPos = puzzleViewPosition.position;
        Quaternion targetRot = puzzleViewPosition.rotation;
        
        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * transitionSpeed;
            
            puzzleCamera.position = Vector3.Lerp(startPos, targetPos, elapsed);
            puzzleCamera.rotation = Quaternion.Slerp(startRot, targetRot, elapsed);
            
            yield return null;
        }
        
        // Ensure final position is exact
        puzzleCamera.position = targetPos;
        puzzleCamera.rotation = targetRot;
        
        isTransitioning = false;
        
        // Update notification
        if (notificationBox != null && notificationText != null)
        {
            notificationText.text = "Press E or ESC to exit";
        }
    }
    
    System.Collections.IEnumerator TransitionCameraToPlayer()
    {
        float elapsed = 0f;
        Vector3 startPos = puzzleCamera.position;
        Quaternion startRot = puzzleCamera.rotation;
        
        // Calculate target world position
        Vector3 targetWorldPos = originalCameraParent.TransformPoint(originalLocalPosition);
        Quaternion targetWorldRot = originalCameraParent.rotation * originalLocalRotation;
        
        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * transitionSpeed;
            
            puzzleCamera.position = Vector3.Lerp(startPos, targetWorldPos, elapsed);
            puzzleCamera.rotation = Quaternion.Slerp(startRot, targetWorldRot, elapsed);
            
            yield return null;
        }
        
        // Re-attach camera to player head
        puzzleCamera.SetParent(originalCameraParent);
        puzzleCamera.localPosition = originalLocalPosition;
        puzzleCamera.localRotation = originalLocalRotation;
        
        isTransitioning = false;
        
        // RE-ENABLE PLAYER CONTROLS
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }
        
        // Lock cursor again for first-person view
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // Hide notification if player left the area
        if (!playerNearby && notificationBox != null)
        {
            notificationBox.SetActive(false);
        }
        else if (notificationText != null)
        {
            notificationText.text = "Press E to examine puzzle";
        }
    }
}
