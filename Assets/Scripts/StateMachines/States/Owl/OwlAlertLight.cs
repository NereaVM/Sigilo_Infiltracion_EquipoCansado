using UnityEngine;

public class OwlAlertLight : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Light owlLight;
    [SerializeField] private VisionSense visionSense;

    [Header("Normal")]
    [SerializeField] private Color normalColor = new Color(187f / 255f, 1f, 0f);
    [SerializeField] private float normalIntensity = 5f;

    [Header("Alert")]
    [SerializeField] private Color alertColor = new Color(1f, 5f / 255f, 0f);
    [SerializeField] private float minAlertIntensity = 1.5f;
    [SerializeField] private float maxAlertIntensity = 5f;
    [SerializeField] private float blinkSpeed = 1.5f;

    [Header("Transition")]
    [SerializeField] private float colorTransitionSpeed = 8f;
    [SerializeField] private float intensityTransitionSpeed = 12f;

    private void Awake()
    {
        if (owlLight == null)
            owlLight = GetComponent<Light>();

        if (owlLight != null)
        {
            owlLight.color = normalColor;
            owlLight.intensity = normalIntensity;
        }
    }

    private void Update()
    {
        if (owlLight == null || visionSense == null)
            return;

        bool seesPlayer = visionSense.hasSeenEntity;

        Color targetColor;
        float targetIntensity;

        if (seesPlayer)
        {
            targetColor = alertColor;

            float blink =
                (Mathf.Sin(Time.time * blinkSpeed * Mathf.PI * 2f) + 1f) * 0.5f;

            targetIntensity = Mathf.Lerp(
                minAlertIntensity,
                maxAlertIntensity,
                blink
            );
        }
        else
        {
            targetColor = normalColor;
            targetIntensity = normalIntensity;
        }

        owlLight.color = Color.Lerp(
            owlLight.color,
            targetColor,
            colorTransitionSpeed * Time.deltaTime
        );

        owlLight.intensity = Mathf.MoveTowards(
            owlLight.intensity,
            targetIntensity,
            intensityTransitionSpeed * Time.deltaTime
        );
    }
}
