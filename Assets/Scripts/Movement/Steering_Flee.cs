using UnityEngine;

public class Steering_Flee : Steering_SeekFlee
{
    public override void OnUpdate()
    {
        rb.AddForce(Flee(target), ForceMode.Acceleration);
    }


    /// <summary>
    /// The Flee behaviour receives a target position and tries to move away from it at max speed.
    /// </summary>
    public Vector3 Flee(Vector3 targetPosition)
    {
        Vector3 desiredVelocityVector = new Vector3(rb.position.x - targetPosition.x, 0, rb.position.z - targetPosition.z);
        return GetSteering(desiredVelocityVector);
    }
}