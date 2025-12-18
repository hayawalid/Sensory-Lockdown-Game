using UnityEngine;
using TMPro;

public class TableInteraction : MonoBehaviour
{
    [Header("UI References")]
    public GameObject notificationBox;
    public TextMeshProUGUI notificationText;
    public GameObject replayButton;   // ✅ Replay button reference

    [Header("Puzzle Scene")]
    public Transform puzzleCamera;
    public float transitionSpeed = 5f;
    public VialPuzzleManager puzzleManager;

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

    // ✅ Store the clicked vial here
    private VialClickMover selectedVial;

    void Start()
    {
        if (notificationBox != null)
            notificationBox.SetActive(false);

        if (replayButton != null)
            replayButton.SetActive(false);   // ✅ Hide replay button at start

        playerMovement = FindObjectOfType<PlayerMovement>();

        if (puzzleCamera != null)
        {
            originalCameraParent = puzzleCamera.parent;
            originalLocalPosition = puzzleCamera.localPosition;
            originalLocalRotation = puzzleCamera.localRotation;
        }
    }

    void Update()
    {
        // ENTER PUZZLE MODE
        if (playerNearby && !isPuzzleView && !isTransitioning)
        {
            if (Input.GetKeyDown(interactKey))
                EnterPuzzleView();
        }

        // PUZZLE MODE ACTIVE
        if (isPuzzleView && !isTransitioning)
        {
            // EXIT PUZZLE MODE
            if (Input.GetKeyDown(exitKey) || Input.GetKeyDown(interactKey))
                ExitPuzzleView();

            // CAMERA MOVEMENT
            float moveSpeed = 1.5f;
            float moveX = Input.GetAxis("Horizontal");
            float moveZ = Input.GetAxis("Vertical");

            Vector3 move = puzzleCamera.right * moveX + puzzleCamera.forward * moveZ;
            puzzleCamera.position += move * moveSpeed * Time.deltaTime;

            // ✅ CLICK TO SELECT A VIAL
            if (Input.GetMouseButtonDown(0))
            {
                Camera cam = puzzleCamera.GetComponent<Camera>();
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 3f))
                {
                    VialClickMover mover = hit.collider.GetComponent<VialClickMover>();

                    if (mover != null)
                    {
                        selectedVial = mover;   // ✅ store clicked vial
                        mover.ResetTilt();

                        mover.ToggleMove();     // optional: your up/down movement
                    }
                }
            }

            // ✅ TILT ONLY THE SELECTED VIAL
            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (selectedVial != null)
                {
                    selectedVial.Tilt();
                    puzzleManager.PlayerSelected(selectedVial);   // ✅ FIXED braces
                }
            }

            // ✅ RESET ONLY THE SELECTED VIAL
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (selectedVial != null)
                    selectedVial.ResetTilt();
            }
        }
    }

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

            if (!isPuzzleView && notificationBox != null)
                notificationBox.SetActive(false);
        }
    }

    void EnterPuzzleView()
    {
        isPuzzleView = true;
        isTransitioning = true;

        if (playerMovement != null)
            playerMovement.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (replayButton != null)
            replayButton.SetActive(true);   // ✅ Show replay button in puzzle mode

        if (puzzleManager != null)
            puzzleManager.ReplaySequence();

        puzzleCamera.SetParent(null);
        StartCoroutine(TransitionCameraToPuzzle());
    }

    void ExitPuzzleView()
    {
        isPuzzleView = false;
        isTransitioning = true;

        if (replayButton != null)
            replayButton.SetActive(false);   // ✅ Hide replay button when leaving puzzle

        StartCoroutine(TransitionCameraToPlayer());
    }

    System.Collections.IEnumerator TransitionCameraToPuzzle()
    {
        float elapsed = 0f;
        Vector3 startPos = puzzleCamera.position;
        Quaternion startRot = puzzleCamera.rotation;

        float distance = 1.1f;
        Vector3 forward = transform.forward;

        Vector3 targetPos = transform.position - forward * distance + Vector3.up * 0.35f;
        Quaternion targetRot = Quaternion.LookRotation(transform.position - targetPos);

        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * transitionSpeed;
            puzzleCamera.position = Vector3.Lerp(startPos, targetPos, elapsed);
            puzzleCamera.rotation = Quaternion.Slerp(startRot, targetRot, elapsed);
            yield return null;
        }

        isTransitioning = false;
        notificationText.text = "Press E or ESC to exit";
    }

    System.Collections.IEnumerator TransitionCameraToPlayer()
    {
        float elapsed = 0f;
        Vector3 startPos = puzzleCamera.position;
        Quaternion startRot = puzzleCamera.rotation;

        Vector3 targetWorldPos = originalCameraParent.TransformPoint(originalLocalPosition);
        Quaternion targetWorldRot = originalCameraParent.rotation * originalLocalRotation;

        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * transitionSpeed;
            puzzleCamera.position = Vector3.Lerp(startPos, targetWorldPos, elapsed);
            puzzleCamera.rotation = Quaternion.Slerp(startRot, targetWorldRot, elapsed);
            yield return null;
        }

        puzzleCamera.SetParent(originalCameraParent);
        puzzleCamera.localPosition = originalLocalPosition;
        puzzleCamera.localRotation = originalLocalRotation;

        isTransitioning = false;

        if (playerMovement != null)
            playerMovement.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (!playerNearby)
            notificationBox.SetActive(false);
        else
            notificationText.text = "Press E to examine puzzle";
    }
}
