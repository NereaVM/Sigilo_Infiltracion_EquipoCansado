
using UnityEngine;

public class Steering_Wander : Steering_Face
{
    // Holds the radius and forward offset of the wander circle.
    [Header("Wander")]
    public float wanderOffset = 3f;
    public float wanderRadius = 1.5f;

    // Maximum rate at which the wander orientation can change.
    public float wanderRate = 45f;

    // Current orientation of the wander target.
    private float wanderOrientation = 0f;

    // Maximum movement speed.
    public float maxWanderSpeed = 2f;

    // Calculated wander target for debugging.
    private Vector3 wanderTarget;

    void Start()
    {
        rb.maxLinearVelocity = maxWanderSpeed;
    }

    public override void OnUpdate()
    {
        SteeringOutput steering = GetSteering();

        // Apply linear acceleration.
        rb.AddForce(
            steering.linear,
            ForceMode.Acceleration
        );

        // Apply angular acceleration.
        rb.AddTorque(
            Vector3.up * steering.angular,
            ForceMode.Acceleration
        );
    }

    public override SteeringOutput GetSteering()
    {
        // 1. Calculate the target to delegate to Face.

        // Update the wander orientation using randomBinomial.
        wanderOrientation +=
            (Random.value - Random.value)
            * wanderRate
            * Time.fixedDeltaTime;

        // Calculate the combined target orientation.
        float targetOrientationWander =
            wanderOrientation + rb.rotation.eulerAngles.y;

        // Get the character's forward direction.
        Vector3 forward =
            rb.rotation * Vector3.forward;

        // Calculate the center of the wander circle.
        Vector3 circleCenter =
            rb.position + wanderOffset * forward;

        // Calculate the target location.
        Vector3 targetDirection =
            Quaternion.Euler(
                0f,
                targetOrientationWander,
                0f
            ) * Vector3.forward;

        wanderTarget =
            circleCenter + wanderRadius * targetDirection;

        // 2. Delegate to Face.
        SteeringOutput steering =
            FacePosition(wanderTarget);

        // 3. Apply full acceleration along the orientation.
        steering.linear =
            maxSteeringForce * forward;

        // Return the steering output.
        return steering;
    }


private void OnDrawGizmos()
{
    if (!Application.isPlaying || !isActiveAndEnabled || rb == null)
        return;

    // Calculate the center of the wander circle.
    Vector3 forward = rb.rotation * Vector3.forward;

    Vector3 circleCenter =
        rb.position + wanderOffset * forward;

    // Draw the wander circle.
    Gizmos.color = Color.cyan;

    const int segments = 32;

    Vector3 previousPoint =
        circleCenter + Vector3.right * wanderRadius;

    for (int i = 1; i <= segments; i++)
    {
        float angle = i * Mathf.PI * 2f / segments;

        Vector3 nextPoint = circleCenter + new Vector3(
            Mathf.Cos(angle) * wanderRadius,
            0f,
            Mathf.Sin(angle) * wanderRadius
        );

        Gizmos.DrawLine(previousPoint, nextPoint);

        previousPoint = nextPoint;
    }

    // Draw the line towards the wander target.
    Gizmos.color = Color.blue;

    Gizmos.DrawLine(
        rb.position,
        wanderTarget
    );

    // Draw the calculated wander target.
    Gizmos.color = Color.yellow;

    Gizmos.DrawSphere(wanderTarget, 0.12f);

    // Draw the current velocity.
    Gizmos.color = Color.green;

    Gizmos.DrawLine(
        rb.position,
        rb.position + rb.linearVelocity
    );
}

}
