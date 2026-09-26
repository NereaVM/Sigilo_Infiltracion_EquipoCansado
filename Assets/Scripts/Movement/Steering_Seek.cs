using UnityEngine;

public class Steering_Seek : Steering_SeekFlee
{
    public override void OnUpdate()
    {
        rb.AddForce(Seek(target), ForceMode.Acceleration);
    }

    /// <summary>
    /// The Seek behaviour receives a target position and tries to move towards it at max speed.
    /// </summary>
    public Vector3 Seek(Vector3 targetPosition)
    {
        Vector3 desiredVelocityVector = new Vector3(targetPosition.x - rb.position.x, 0, targetPosition.z - rb.position.z);
        return GetSteering(desiredVelocityVector);
    }
}