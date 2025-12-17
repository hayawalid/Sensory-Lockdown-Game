using UnityEngine;

public class VialClickMover : MonoBehaviour
{
    // UP/DOWN MOVEMENT
    private bool isUp = false;
    public float moveAmount = 0.2f;
    public float moveSpeed = 5f;

    private Vector3 originalPos;
    private Vector3 targetPos;

    // TILTING
    [Header("Tilt Settings")]
    public float tiltAngle = 15f; // positive = right, negative = left

    // FADE EFFECT
    private VialPour pourScript;

    void Start()
    {
        originalPos = transform.position;
        targetPos = originalPos;

        // ✅ Find the VialPour script on this vial
        pourScript = GetComponentInChildren<VialPour>();
    }

    // CLICK = MOVE UP/DOWN
    public void ToggleMove()
    {
        isUp = !isUp;

        if (isUp)
            targetPos = originalPos + Vector3.up * moveAmount;
        else
            targetPos = originalPos;
    }

    // Q = TILT + FADE
    public void Tilt()
    {
        transform.localRotation = Quaternion.Euler(0, 0, tiltAngle);

        if (pourScript != null)
            pourScript.StartPouring();
    }

    // R = RESET + STOP FADE
    public void ResetTilt()
    {
        transform.localRotation = Quaternion.identity;

        if (pourScript != null)
            pourScript.StopPouring();
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * moveSpeed);
    }
}
