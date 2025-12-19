using UnityEngine;
using TMPro;
using System.Collections;

public class SuccessAnimation : MonoBehaviour
{
    [Header("References")]
    public BowlFlash bowlFlash;              // ✅ your bowl flash script
    public ParticleSystem successParticles;  // ✅ particle burst
    public Transform cameraTarget;           // ✅ puzzle camera
    public AudioSource audioSource;          // ✅ audio source
    public AudioClip successSound;           // ✅ success sound
    

    [Header("Camera Shake Settings")]
    public float shakeIntensity = 0.1f;
    public float shakeDuration = 0.3f;

    public void PlaySuccess()
    {
        // ✅ Bowl flash
        if (bowlFlash != null)
            bowlFlash.Flash();

        // ✅ Particle burst
        if (successParticles != null)
            successParticles.Play();

        // ✅ Sound
        if (audioSource != null && successSound != null)
            audioSource.PlayOneShot(successSound);

        // ✅ UI popup
       

        // ✅ Camera shake
        StartCoroutine(ShakeCamera());
    }

    private IEnumerator ShakeCamera()
    {
        if (cameraTarget == null) yield break;

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
