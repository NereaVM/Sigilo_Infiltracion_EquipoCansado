using UnityEngine;

public abstract class Steering_SeekFlee : MovementScript
{
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        OnUpdate();
    }

    public abstract void OnUpdate();


    public Vector3 GetSteering(Vector3 desiredVelocityVector)
    {
        // We want to move in the direction of the desired velocity vector at max speed,
        // so we change the magnitude of the vector between the target and current positions
        // to our max speed, while keeping the direction.
        desiredVelocityVector.Normalize();
        desiredVelocityVector *= rb.maxLinearVelocity;

        // The steering vector is the difference between the desired velocity and the current velocity.
        // Changing the amount of force applied to the object will change how quickly it can change direction and achive max speed again.
        Vector3 steeringVector = desiredVelocityVector - rb.linearVelocity;
        steeringVector.Normalize();
        steeringVector *= maxSteeringForce;

        return steeringVector;
    }

}