using UnityEngine;

public class OwlInvestigationState : State
{
    [SerializeField] private HearingSense_Owl hearingSense;
    [SerializeField] private Transform visionPivot;

    [SerializeField] private float rotationSpeed = 120f;

    private Vector3 soundPosition;

    public override void OnEntry()
    {
        soundPosition = hearingSense.lastSoundPosition;

        foreach (Transition transition in transitions)
        {
            if (transition is Transition_Timer timer)
            {
                timer.ResetTimer();
            }
        }
    }

    public override void OnUpdate()
    {
        if (visionPivot == null)
            return;

        Vector3 direction =
            soundPosition - visionPivot.position;

        direction.y = 0f;

        if (direction.sqrMagnitude <= 0.001f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(direction.normalized);

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