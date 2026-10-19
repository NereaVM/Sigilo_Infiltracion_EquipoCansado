using UnityEngine;

public class OwlAlertState : State
{
    [Header("References")]
    [SerializeField] private VisionSense visionSense;
    [SerializeField] private HearingSense_Owl hearingSense;
    [SerializeField] private Transform visionPivot;

    [Header("Alert Sound")]
    [SerializeField] private GeneratedSound alertSoundPrefab;

    [Header("Vision Tracking")]
    [SerializeField] private float rotationSpeed = 120f;

    public override void OnEntry()
    {
        // Reinicia el temporizador del estado de alerta.
        foreach (Transition transition in transitions)
        {
            if (transition is Transition_Timer timer)
            {
                timer.ResetTimer();
            }
        }

        // Genera una única alerta al entrar en el estado.
        if (alertSoundPrefab != null)
        {
            GeneratedSound alertSound =
                Instantiate(
                    alertSoundPrefab,
                    transform.root.position,
                    Quaternion.identity
                );

            // Evita que el búho escuche su propia alerta.
            alertSound.soundGenerator = hearingSense;
        }
    }

    public override void OnUpdate()
    {
        // Mientras siga viendo al jugador,
        // mantiene la mirada dirigida hacia él.
        if (visionSense == null ||
            visionPivot == null ||
            !visionSense.hasSeenEntity)
        {
            return;
        }

        Vector3 direction =
            visionSense.lastSeenPosition -
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