using UnityEngine;

public class OwlAlertState : State
{
    [Header("References")]
    [SerializeField] private VisionSense owlVisionSense;
    [SerializeField] private HearingSense_Owl owlHearingSense;
    [SerializeField] private Transform visionPivot;

    [Header("Alert Sound - AI")]
    [SerializeField] private GeneratedSound alertSoundPrefab;

    [Header("Alert Sound - Audio")]
    [SerializeField] private AudioClip alertAudioClip;

    [Range(0f, 1f)]
    [SerializeField] private float alertVolume = 1f;

    [Header("Vision Tracking")]
    [SerializeField] private float rotationSpeed = 120f;

    public override void OnEntry()
    {
        // Reiniciar temporizadores del estado.
        foreach (Transition transition in transitions)
        {
            if (transition is Transition_Timer timer)
            {
                timer.ResetTimer();
            }
        }

        // Sonido lógico para la IA.
        if (alertSoundPrefab != null)
        {
            GeneratedSound alertSound =
                Instantiate(
                    alertSoundPrefab,
                    transform.root.position,
                    Quaternion.identity
                );

            // Evita que el búho escuche su propia alerta.
            alertSound.soundGenerator = owlHearingSense;
        }

        // Sonido real para el jugador.
        // Se reproduce una sola vez al entrar en Alert.
        if (alertAudioClip != null)
        {
            AudioSource.PlayClipAtPoint(
                alertAudioClip,
                transform.root.position,
                alertVolume
            );
        }
    }

    public override void OnUpdate()
    {
        if (owlVisionSense == null ||
            visionPivot == null ||
            !owlVisionSense.hasSeenEntity)
        {
            return;
        }

        Vector3 direction =
            owlVisionSense.lastSeenPosition -
            visionPivot.position;

        direction.y = 0f;

        if (direction.sqrMagnitude <= 0.001f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(
                direction.normalized,
                Vector3.up
            );

        visionPivot.rotation =
            Quaternion.RotateTowards(
                visionPivot.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
    }

    public override void OnExit()
    {
    }
}