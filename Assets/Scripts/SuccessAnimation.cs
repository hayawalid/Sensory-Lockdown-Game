using UnityEngine;
using TMPro;

public class SuccessAnimation : MonoBehaviour
{
    [Header("Bowl Flash")]
    public BowlFlash bowlFlash;

    [Header("Particles")]
    public ParticleSystem successParticles;

    [Header("Camera Shake")]
    public Transform cameraTarget;
    public float shakeIntensity = 0.1f;
    public float shakeDuration = 0.3f;

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip successSound;

    [Header("UI")]
    public GameObject successPanel;
    public TextMeshProUGUI successText;

    public void PlaySuccess()
    {
        if (bowlFlash != null)
            bowlFlash.Flash();

        if (successParticles != null)
            successParticles.Play();

        if (audioSource != null && successSound != null)
            audioSource.PlayOneShot(successSound);

        if (successPanel != null)
        {
            successPanel.SetActive(true);
            successText.text = "Success!";
        }

        StartCoroutine(ShakeCamera());
    }

    System.Collections.IEnumerator ShakeCamera()
    {
        if (cameraTarget == null)
            yield break;

        Vector3 originalPos = cameraTarget.localPosition;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;
            cameraTarget.localPosition = originalPos + Random.insideUnitSphere * shakeIntensity;
            yield return null;
        }

        cameraTarget.localPosition = originalPos;
    }
}
