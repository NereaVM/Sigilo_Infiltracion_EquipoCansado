using UnityEngine;

public class Steering_Arrive : MovementScript
{
    public float arriveDistance = 1f;
    public float slowingDistance = 5f;
    public float timeToTargetSpeed = 0.1f;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        OnUpdate();
    }

    public void OnUpdate()
    {
        rb.AddForce(Arrive(target), ForceMode.Acceleration);
    }

    /// <summary>
    /// The Arrive behaviour receives a target position and tries to move towards it at max speed, but slows down as it approaches the target.
    /// A sufficiently strong steering force is still needed to avoid overshooting.
    /// </summary>
    public Vector3 Arrive(Vector3 targetPosition)
    {
        Vector3 desiredVelocityVector = new Vector3(targetPosition.x - rb.position.x, 0, targetPosition.z - rb.position.z);
        float distanceToTarget = desiredVelocityVector.magnitude;

        float targetSpeed;

        // Determine the target speed based on the distance to the target and the slowing distance:
            // Stop moving when within the arrive distance
        if (distanceToTarget < arriveDistance)
        {
            targetSpeed = 0f;
        }
            // Slow down when within the slowing distance
        else if (distanceToTarget < slowingDistance)
        {
            targetSpeed = rb.maxLinearVelocity * (distanceToTarget / slowingDistance);
        }
            // Move at max speed when outside the slowing distance
        else
        {
            targetSpeed = rb.maxLinearVelocity;
        }

        // Calculate the desired velocity vector based on the target speed and direction
        desiredVelocityVector.Normalize();
        desiredVelocityVector *= targetSpeed;

        // Get the steering vector and ensure it is reached within the time to target speed
        Vector3 steeringVector = desiredVelocityVector - rb.linearVelocity;
        steeringVector /= timeToTargetSpeed;

        // Clamp the steering force
        if (steeringVector.magnitude > maxSteeringForce)
        {
            steeringVector.Normalize();
            steeringVector *= maxSteeringForce;
        }

        return steeringVector;
    }
}