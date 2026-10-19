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
    }

    public override void OnUpdate()
    {
        if (visionPivot == null || lookPoints.Length == 0)
            return;

        Vector3 direction =
            lookPoints[currentPoint].position -
            visionPivot.position;

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
    }

    public override void OnExit()
    {
    }
}