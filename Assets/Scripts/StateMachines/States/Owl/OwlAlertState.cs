using UnityEngine;

public class OwlAlertState : State
{
    [Header("References")]
    [SerializeField] private VisionSense owlVisionSense;
    [SerializeField] private HearingSense_Owl owlHearingSense;
    [SerializeField] private Transform visionPivot;

    [Header("Alert Sound")]
    [SerializeField] private GeneratedSound alertSoundPrefab;

    [Header("Vision Tracking")]
    [SerializeField] private float rotationSpeed = 120f;

    public override void OnEntry()
    {
        foreach (Transition transition in transitions)
        {
            if (transition is Transition_Timer timer)
            {
                timer.ResetTimer();
            }
        }

        if (alertSoundPrefab != null)
        {
            GeneratedSound alertSound =
                Instantiate(
                    alertSoundPrefab,
                    transform.root.position,
                    Quaternion.identity
                );

            alertSound.soundGenerator = owlHearingSense;
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
