using UnityEngine;

public class Crocodile_Pursue : Steering_Pursue
{
    private float currentForceCap = 0f;
    public float minSteeringForce = 0.1f; // The minimum steering force that the crocodile can apply
    private float accelerationDuration = 0f; // How long the crocodile has been accelerating
    public float TimeToMaxForce = 2f; // The time it takes for the crocodile to reach its maximum force
    public float accelerationMaxAngle; // The maximum angle between the current velocity and the target position that allows for acceleration

    // Applies the custom acceleration system of the crocodile to the Seek behaviour.
    public override void OnUpdate()
    {
        Vector3 steering = Pursue(target);
        float currentAngle = Vector3.Angle(rb.linearVelocity, target - rb.position);
        if (currentAngle < accelerationMaxAngle)
        {
            accelerationDuration += Time.deltaTime;

            currentForceCap = CalculateCurrentForceCap(accelerationDuration); 
        }
        else if (currentAngle < 100f)
        {
            accelerationDuration = 0f;
            currentForceCap = Mathf.Lerp(minSteeringForce/2, maxSteeringForce/2, currentAngle/100f);
        }
        else
        {
            accelerationDuration = 0f;
            currentForceCap = maxSteeringForce;
        }
        steering = Vector3.ClampMagnitude(steering, currentForceCap); // Clamp the steering force to the current force cap

        rb.AddForce(steering, ForceMode.Acceleration);
        Debug.Log($"Current Force Cap: {currentForceCap}, Acceleration Duration: {accelerationDuration}, Current Angle: {currentAngle}, Velocity: {rb.linearVelocity.magnitude}");
    }

    private float CalculateCurrentForceCap(float accelerationDuration)
    {
        float normalizedTime = Mathf.Clamp01(accelerationDuration / TimeToMaxForce);

        float multiplier = maxSteeringForce * Mathf.Pow(normalizedTime, 5);// + normalizedTime/5); // Starts slow for a while, accelerates suddenly
        multiplier = Mathf.Clamp01(multiplier);
        
        return maxSteeringForce * multiplier; // Ensure the force cap is not below the minimum steering force
    }

    void OnDrawGizmos(){
        if (rb == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(rb.position, rb.linearVelocity + rb.position);
    }
}