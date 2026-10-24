using UnityEngine;

public class Steering_Pursue : Steering_Arrive
{
    public float maxPredictionTime = 2f;

    private Rigidbody targetRb;
    private Vector3 targetVelocity = Vector3.zero;
    private Vector3 pursuitTarget = Vector3.zero;
    private Vector3 previousTargetPosition;

    private Transform cachedTargetTransform;

    public override void OnUpdate()
    {
        UpdateTargetReference();

        if (targetTransform != null)
        {
            target = targetTransform.position;
        }

        rb.AddForce(
            Pursue(target),
            ForceMode.Acceleration
        );
    }

    public Vector3 Pursue(Vector3 target)
    {
        GetTargetVelocity();

        // 1. Calculate target to delegate to Arrive.
        pursuitTarget = target;

        // Work out distance to target.
        float distance =
            Vector3.Distance(rb.position, target);

        // Work out current speed.
        float speed =
            rb.linearVelocity.magnitude;

        float arrivalTimePrediction =
            maxPredictionTime;

        // Check if speed is too small to give
        // a reasonable prediction time.
        if (speed > distance / maxPredictionTime)
        {
            arrivalTimePrediction =
                distance / speed;
        }

        // Put the target together.
        pursuitTarget +=
            targetVelocity * arrivalTimePrediction;

        Vector3 steering =
            Arrive(pursuitTarget);

        return steering;
    }

    private void GetTargetVelocity()
    {
        UpdateTargetReference();

        if (targetRb != null)
        {
            if (targetRb.isKinematic)
            {
                targetVelocity =
                    (targetTransform.position -
                     previousTargetPosition)
                    / Time.fixedDeltaTime;
            }
            else
            {
                targetVelocity =
                    targetRb.linearVelocity;
            }
        }
        else
        {
            targetVelocity =
                Vector3.zero;
        }

        if (targetTransform != null)
        {
            previousTargetPosition =
                targetTransform.position;
        }
    }

    private void UpdateTargetReference()
    {
        if (targetTransform == null)
        {
            targetRb = null;
            cachedTargetTransform = null;
            return;
        }

        if (cachedTargetTransform != targetTransform)
        {
            cachedTargetTransform =
                targetTransform;

            targetRb =
                targetTransform.GetComponent<Rigidbody>();

            previousTargetPosition =
                targetTransform.position;
        }
    }

    private void OnDrawGizmos()
    {
        if (rb == null)
            return;

        Gizmos.color = Color.blue;

        Gizmos.DrawLine(
            rb.position,
            rb.position + rb.linearVelocity
        );

        Gizmos.DrawSphere(
            pursuitTarget,
            0.5f
        );
    }
}