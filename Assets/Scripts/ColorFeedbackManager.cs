using UnityEngine;

public class ColorFeedbackManager : MonoBehaviour
{
    [Header("Target Setup")]
    public MeshRenderer filterCubeRenderer; // Drag the Camera's Cube child here

    [Header("Color Restoration Settings")]
    public float redThreshold = 0.33f;
    public float greenThreshold = 0.66f;
    public float blueThreshold = 1.0f;
    public float colorTransitionSpeed = 2.0f;

    private float targetRed, targetGreen, targetBlue;
    private float curRed, curGreen, curBlue;
    private Material instanceMaterial;

    void Start() 
    { 
        // Create an instance so we don't change the actual Project File material
        if (filterCubeRenderer != null)
        {
            instanceMaterial = filterCubeRenderer.material;
        }
        ResetToGrayscale(); 
    }

    void Update()
    {
        if (instanceMaterial == null) return;

        // Smoothly transition current values toward targets
        curRed = Mathf.Lerp(curRed, targetRed, Time.deltaTime * colorTransitionSpeed);
        curGreen = Mathf.Lerp(curGreen, targetGreen, Time.deltaTime * colorTransitionSpeed);
        curBlue = Mathf.Lerp(curBlue, targetBlue, Time.deltaTime * colorTransitionSpeed);

        // Update the Shader properties
        instanceMaterial.SetFloat("_RedMultiplier", curRed);
        instanceMaterial.SetFloat("_GreenMultiplier", curGreen);
        instanceMaterial.SetFloat("_BlueMultiplier", curBlue);
    }

    public void OnCorrectSelection(int count, int total)
    {
        float progress = (float)count / total;
        UpdateTargets(progress);
    }

    public void OnWrongSelection()
    {
        ResetToGrayscale();
    }

    private void UpdateTargets(float progress)
    {
        // Gradually turn on R, then G, then B based on progress
        targetRed = (progress >= redThreshold) ? 1f : 0f;
        targetGreen = (progress >= greenThreshold) ? 1f : 0f;
        targetBlue = (progress >= blueThreshold) ? 1f : 0f;
    }

    public void ResetToGrayscale()
    {
        targetRed = targetGreen = targetBlue = 0f;
        // If we want it to snap back to gray immediately on fail:
        // curRed = curGreen = curBlue = 0f; 
    }
}