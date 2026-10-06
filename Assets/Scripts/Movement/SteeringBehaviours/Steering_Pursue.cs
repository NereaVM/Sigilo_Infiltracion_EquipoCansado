using UnityEngine;

public class Steering_Pursue : Steering_Arrive
{
    public float maxPredictionTime = 2f;
    private Rigidbody targetRb;
    private Vector3 targetVelocity = Vector3.zero;
    private Vector3 pursuitTarget = Vector3.zero;
    
    public override void OnUpdate()
    {
        rb.AddForce(Pursue(target), ForceMode.Acceleration);
    }

    public Vector3 Pursue(Vector3 target)
    {
        GetTargetVelocity();
        // 1. Calculate target to delegate to Arrive 
        // Work out distance to target
        pursuitTarget = target;
        float distance = Vector3.Distance(rb.position, target);

        // Work out current speed
        float speed = rb.linearVelocity.magnitude;

        float arrivalTimePrediction = maxPredictionTime;
        // Check if speed is too small to give a reasonable prediction time
        if (speed > distance / maxPredictionTime)
        {
            arrivalTimePrediction = distance/speed;
        }

        // Put the target together
        pursuitTarget += targetVelocity * arrivalTimePrediction;
        Vector3 steering = Arrive(pursuitTarget);

        return steering;
    }

    private void GetTargetVelocity()
    {
        if (targetRb != null)
        {
            targetVelocity = targetRb.linearVelocity;
            Debug.Log("SUCCESS");
        }
        else
        {
            targetVelocity = Vector3.zero;
        }
        Debug.Log($"Target Velocity: {targetVelocity.magnitude}, Current Target Position: {target}");
    }

    void OnDrawGizmos()
    {
        if (rb == null) return;
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(rb.position, rb.linearVelocity + rb.position);
        Gizmos.DrawSphere(pursuitTarget, 0.5f);
    }
}