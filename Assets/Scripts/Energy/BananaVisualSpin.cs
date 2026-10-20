using System.Collections;
using UnityEngine;

public class BananaVisualSpin : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform bananaVisual;
    [SerializeField] private Light pointLight;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 90f;

    [Header("Light Pulse")]
    [SerializeField] private float normalIntensity = 1.5f;
    [SerializeField] private float pulseIntensity = 4f;
    [SerializeField] private float pulseDuration = 0.5f;

    private float accumulatedRotation = 0f;
    private bool isPulsing = false;

    private void Awake()
    {
        if (pointLight != null)
        {
            pointLight.intensity = normalIntensity;
        }
    }

    private void Update()
    {
        if (bananaVisual == null)
            return;

        float rotationThisFrame = rotationSpeed * Time.deltaTime;

        // El pivote gira solo sobre su eje Y
        bananaVisual.Rotate(
            0f,
            rotationThisFrame,
            0f,
            Space.Self
        );

        accumulatedRotation += Mathf.Abs(rotationThisFrame);

        if (accumulatedRotation >= 360f)
        {
            accumulatedRotation -= 360f;

            if (!isPulsing && pointLight != null)
            {
                StartCoroutine(PulseLight());
            }
        }
    }

    private IEnumerator PulseLight()
    {
        isPulsing = true;

        float halfDuration = pulseDuration * 0.5f;

        // Subida suave.
        float timer = 0f;

        while (timer < halfDuration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / halfDuration);
            t = Mathf.SmoothStep(0f, 1f, t);

            pointLight.intensity = Mathf.Lerp(
                normalIntensity,
                pulseIntensity,
                t
            );

            yield return null;
        }

        // Bajada suave.
        timer = 0f;

        while (timer < halfDuration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / halfDuration);
            t = Mathf.SmoothStep(0f, 1f, t);

            pointLight.intensity = Mathf.Lerp(
                pulseIntensity,
                normalIntensity,
                t
            );

            yield return null;
        }

        pointLight.intensity = normalIntensity;
        isPulsing = false;
    }
}