using UnityEngine;

public class VialGlow : MonoBehaviour
{
    private Renderer rend;
    private Material mat;
    private Color originalColor;

    public Color glowColor = Color.yellow;
    public float glowIntensity = 3f;

    void Start()
    {
        rend = GetComponentInChildren<Renderer>();
        if (rend == null)
        {
            Debug.LogError("VialGlow: No Renderer found on " + gameObject.name);
            return;
        }

        mat = rend.material;
        if (mat.HasProperty("_EmissionColor"))
        {
            originalColor = mat.GetColor("_EmissionColor");
        }
        else
        {
            Debug.LogWarning("VialGlow: Material on " + gameObject.name + " has no _EmissionColor property.");
            originalColor = Color.black;
        }
    }

    public void GlowOn()
    {
        if (mat == null) return;

        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", glowColor * glowIntensity);
    }

    public void GlowOff()
    {
        if (mat == null) return;

        mat.SetColor("_EmissionColor", originalColor);
    }
}
