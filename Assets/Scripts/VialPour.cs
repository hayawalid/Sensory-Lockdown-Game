using UnityEngine;

public class VialPour : MonoBehaviour
{
    [Header("Assign the LiquidScaler object here")]
    public Transform liquidScaler;

    public float pourSpeed = 0.3f;
    public float minFill = 0.0f;
    public float maxFill = 1.0f;

    private float fillAmount = 1.0f;
    private bool isPouring = false;

    void Update()
    {
        if (!isPouring || liquidScaler == null)
            return;

        // ✅ Keep liquid upright relative to vial
        liquidScaler.localRotation = Quaternion.identity;

        // ✅ Fade effect (scale down)
        fillAmount -= pourSpeed * Time.deltaTime;
        fillAmount = Mathf.Clamp(fillAmount, minFill, maxFill);

        Vector3 scale = liquidScaler.localScale;
        scale.y = fillAmount;
        liquidScaler.localScale = scale;
    }

    // ✅ Called by VialClickMover
    public void StartPouring()
    {
        isPouring = true;
    }

    // ✅ Called by VialClickMover
    public void StopPouring()
    {
        isPouring = false;

        // Optional: refill instantly
        fillAmount = 1f;

        if (liquidScaler != null)
        {
            Vector3 scale = liquidScaler.localScale;
            scale.y = fillAmount;
            liquidScaler.localScale = scale;
        }
    }
}
