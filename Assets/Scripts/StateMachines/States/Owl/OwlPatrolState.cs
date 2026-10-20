using UnityEngine;

public class OwlPatrolState : State
{
    [Header("Vision Patrol")]
    [SerializeField] private Transform visionPivot;
    [SerializeField] private Transform[] lookPoints;

    [SerializeField] private float rotationSpeed = 90f;
    [SerializeField] private float waitTime = 2f;

    private int currentPoint = 0;
    private float timer = 0f;

    public override void OnEntry()
    {
        timer = 0f;

        // For the black owl, restart the relocation timer
        // every time it returns to Patrol.
        foreach (Transition transition in transitions)
        {
            if (transition is Transition_OwlRelocate relocateTransition)
            {
                relocateTransition.ResetTimer();
            }
        }
    }

    public override void OnUpdate()
    {
        if (visionPivot == null ||
            lookPoints == null ||
            lookPoints.Length == 0)
        {
            return;
        }

        Transform target = lookPoints[currentPoint];

        if (target == null)
            return;

        Vector3 direction =
            target.position - visionPivot.position;

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

        float angle =
            Quaternion.Angle(
                visionPivot.rotation,
                targetRotation
            );

        if (angle < 2f)
        {
            timer += Time.deltaTime;

            if (timer >= waitTime)
            {
                timer = 0f;

                currentPoint =
                    (currentPoint + 1) %
                    lookPoints.Length;
            }
        }
        else
        {
            timer = 0f;
        }
    }

    public override void OnExit()
    {
    }
}
