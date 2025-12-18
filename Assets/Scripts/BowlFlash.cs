using UnityEngine;

public class BowlFlash : MonoBehaviour
{
    private Material mat;
    private Color baseEmission;
    public float flashIntensity = 5f;
    public float flashDuration = 0.5f;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
        baseEmission = mat.GetColor("_EmissionColor");
    }

    public void Flash()
    {
        StopAllCoroutines();
        StartCoroutine(FlashRoutine());
    }

    System.Collections.IEnumerator FlashRoutine()
    {
        float t = 0f;

        // Flash up
        while (t < 1f)
        {
            t += Time.deltaTime / flashDuration;
            float intensity = Mathf.Lerp(0f, flashIntensity, t);
            mat.SetColor("_EmissionColor", Color.white * intensity);
            yield return null;
        }

        t = 0f;

        // Fade down
        while (t < 1f)
        {
            t += Time.deltaTime / flashDuration;
            float intensity = Mathf.Lerp(flashIntensity, 0f, t);
            mat.SetColor("_EmissionColor", Color.white * intensity);
            yield return null;
        }

        // Reset
        mat.SetColor("_EmissionColor", baseEmission);
    }
}
