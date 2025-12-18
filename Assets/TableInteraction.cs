using UnityEngine;
using TMPro;
using System.Collections;

public class TableInteraction : MonoBehaviour
{
    [Header("UI References")]
    public GameObject notificationBox;
    public TextMeshProUGUI notificationText;
    public GameObject replayButton;

    [Header("Puzzle Scene")]
    public Transform puzzleCamera;
    public float transitionSpeed = 5f;
    public VialPuzzleManager puzzleManager;
    public ColorFeedbackManager colorFeedback;

    [Header("Filter Setup")]
    public MeshRenderer filterCubeRenderer; 
    public float puzzleSaturation = 0.3f;

    [Header("Settings")]
    public KeyCode interactKey = KeyCode.E;
    public KeyCode exitKey = KeyCode.Escape;

    private bool playerNearby = false;
    private bool isPuzzleView = false;
    private bool isTransitioning = false;

    private Transform originalCameraParent;
    private Vector3 originalLocalPosition;
    private Quaternion originalLocalRotation;
    private PlayerMovement playerMovement;
    private VialClickMover selectedVial;

    void Start()
    {
        if (notificationBox != null) notificationBox.SetActive(false);
        if (replayButton != null) replayButton.SetActive(false);
        if (filterCubeRenderer != null) filterCubeRenderer.enabled = false;

        playerMovement = Object.FindFirstObjectByType<PlayerMovement>();

        if (colorFeedback == null)
            colorFeedback = Object.FindFirstObjectByType<ColorFeedbackManager>();

        if (puzzleCamera != null)
        {
            originalCameraParent = puzzleCamera.parent;
            originalLocalPosition = puzzleCamera.localPosition;
            originalLocalRotation = puzzleCamera.localRotation;
        }
    }

    void Update()
    {
        if (playerNearby && !isPuzzleView && !isTransitioning)
        {
            if (Input.GetKeyDown(interactKey))
                EnterPuzzleView();
        }

        if (isPuzzleView && !isTransitioning)
        {
            // Exit logic
            if (Input.GetKeyDown(exitKey)) 
                ExitPuzzleView();

            // --- CAMERA MOVEMENT ---
            float moveSpeed = 1.5f;
            float moveX = Input.GetAxis("Horizontal");
            float moveZ = Input.GetAxis("Vertical");
            Vector3 move = puzzleCamera.right * moveX + puzzleCamera.forward * moveZ;
            puzzleCamera.position += move * moveSpeed * Time.deltaTime;

            // --- VIAL SELECTION (MOUSE) ---
            if (Input.GetMouseButtonDown(0))
            {
                Camera cam = puzzleCamera.GetComponent<Camera>();
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 10f))
                {
                    VialClickMover mover = hit.collider.GetComponent<VialClickMover>();
                    if (mover != null)
                    {
                        // Reset previous vial if necessary, then select new one
                        if (selectedVial != null && selectedVial != mover) 
                            selectedVial.ResetTilt();

                        selectedVial = mover;
                        mover.ToggleMove();
                    }
                }
            }

            // --- POUR LIQUID (Q) ---
            if (Input.GetKeyDown(KeyCode.Q) && selectedVial != null)
            {
                selectedVial.Tilt();
                if (puzzleManager != null)
                    puzzleManager.PlayerSelected(selectedVial);
            }

            // --- RESET VIAL (R) ---
            if (Input.GetKeyDown(KeyCode.R) && selectedVial != null)
            {
                selectedVial.ResetTilt();
            }
        }
    }

    void EnterPuzzleView()
    {
        isPuzzleView = true;
        isTransitioning = true;

        if (filterCubeRenderer != null) filterCubeRenderer.enabled = true;
        if (playerMovement != null) playerMovement.enabled = false;
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (replayButton != null) replayButton.SetActive(true);

        // Tell puzzle manager to show sequence when entering
        if (puzzleManager != null)
            puzzleManager.ReplaySequence();

        puzzleCamera.SetParent(null);
        StartCoroutine(TransitionCameraToPuzzle());
    }

    void ExitPuzzleView()
    {
        isPuzzleView = false;
        isTransitioning = true;

        if (filterCubeRenderer != null) filterCubeRenderer.enabled = false;
        if (replayButton != null) replayButton.SetActive(false);

        StartCoroutine(TransitionCameraToPlayer());
    }

    public void UpdateFilter(float red, float green, float blue, float satValue)
    {
        if (filterCubeRenderer != null && filterCubeRenderer.material != null)
        {
            filterCubeRenderer.material.SetFloat("_RedMultiplier", red);
            filterCubeRenderer.material.SetFloat("_GreenMultiplier", green);
            filterCubeRenderer.material.SetFloat("_BlueMultiplier", blue);
            filterCubeRenderer.material.SetFloat("_Saturation", satValue);
        }
    }

    // --- TRANSITION COROUTINES ---

    IEnumerator TransitionCameraToPuzzle()
    {
        float elapsed = 0f;
        Vector3 startPos = puzzleCamera.position;
        Quaternion startRot = puzzleCamera.rotation;

        float distance = 1.1f;
        Vector3 forward = transform.forward;
        
        // Using the "LookAtPoint" logic from your merge
        Vector3 lookAtPoint = transform.position - transform.right * 1.5f;
        Vector3 targetPos = lookAtPoint - forward * distance + Vector3.up * 0.35f;
        Quaternion targetRot = Quaternion.LookRotation(lookAtPoint - targetPos);

        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * transitionSpeed;
            puzzleCamera.position = Vector3.Lerp(startPos, targetPos, elapsed);
            puzzleCamera.rotation = Quaternion.Slerp(startRot, targetRot, elapsed);
            yield return null;
        }

        isTransitioning = false;
        if (notificationText != null) notificationText.text = "Press ESC to exit";
    }

    IEnumerator TransitionCameraToPlayer()
    {
        float elapsed = 0f;
        Vector3 startPos = puzzleCamera.position;
        Quaternion startRot = puzzleCamera.rotation;

        Vector3 targetWorldPos = (originalCameraParent != null) ? 
            originalCameraParent.TransformPoint(originalLocalPosition) : originalLocalPosition;
        Quaternion targetWorldRot = (originalCameraParent != null) ? 
            originalCameraParent.rotation * originalLocalRotation : originalLocalRotation;

        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * transitionSpeed;
            puzzleCamera.position = Vector3.Lerp(startPos, targetWorldPos, elapsed);
            puzzleCamera.rotation = Quaternion.Slerp(startRot, targetWorldRot, elapsed);
            yield return null;
        }

        if (originalCameraParent != null) puzzleCamera.SetParent(originalCameraParent);
        puzzleCamera.localPosition = originalLocalPosition;
        puzzleCamera.localRotation = originalLocalRotation;

        isTransitioning = false;
        if (playerMovement != null) playerMovement.enabled = true;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (!playerNearby && notificationBox != null)
            notificationBox.SetActive(false);
        else if (notificationText != null)
            notificationText.text = "Press E to examine puzzle";
    }

    // --- TRIGGER LOGIC ---

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.GetComponent<PlayerMovement>() != null)
        {
            playerNearby = true;
            if (notificationBox != null)
            {
                notificationBox.SetActive(true);
                notificationText.text = "Press E to examine puzzle";
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.GetComponent<PlayerMovement>() != null)
        {
            playerNearby = false;
            if (!isPuzzleView && notificationBox != null) notificationBox.SetActive(false);
        }
    }
}