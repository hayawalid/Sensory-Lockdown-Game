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
        // Search in this object and all its children for a renderer
        rend = GetComponentInChildren<Renderer>();
        
        if (rend != null)
        {
            // Ensure the material supports Emission before trying to get color
            if (rend.material.HasProperty("_EmissionColor"))
                originalColor = rend.material.GetColor("_EmissionColor");
        }
        else
        {
            Debug.LogError($"[VialGlow] No Renderer found on {gameObject.name} or its children!");
        }
    }

    public void GlowOn()
    {
        if (rend == null) return; // Prevents the NullReferenceException
        
        rend.material.EnableKeyword("_EMISSION");
        rend.material.SetColor("_EmissionColor", glowColor * glowIntensity);
    }

    public void GlowOff()
    {
        if (rend == null) return;
        rend.material.SetColor("_EmissionColor", originalColor);
    }
}