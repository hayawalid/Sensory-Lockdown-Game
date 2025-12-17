using UnityEngine;

public class VialGlow : MonoBehaviour
{
    private Renderer rend;
    private Color originalColor;
    public Color glowColor = Color.yellow;
    public float glowIntensity = 3f;

    void Start()
    {
        rend = GetComponentInChildren<Renderer>();
        originalColor = rend.material.GetColor("_EmissionColor");
    }

    public void GlowOn()
    {
        rend.material.EnableKeyword("_EMISSION");
        rend.material.SetColor("_EmissionColor", glowColor * glowIntensity);
    }

    public void GlowOff()
    {
        rend.material.SetColor("_EmissionColor", originalColor);
    }
}
