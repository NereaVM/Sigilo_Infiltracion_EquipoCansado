using UnityEngine;

public class Steering_Align : SteeringBehaviour
{
    [Header("Angular Movement")]
    public float maxAngularAcceleration = 8f;
    public float maxRotation = 3f;

    [Header("Angles in degrees")]
    public float targetRadius = 2f;
    public float slowRadius = 45f;

    [Header("Time")]
    public float timeToTarget = 0.1f;

    // Orientación deseada, en grados.
    protected float targetOrientation;

    public override void OnUpdate()
    {
        SteeringOutput steering = GetSteering();

        rb.AddTorque(
            Vector3.up * steering.angular,
            ForceMode.Acceleration
        );
    }

    public virtual SteeringOutput GetSteering()
    {
        SteeringOutput steering = new SteeringOutput();

        // Orientación actual del personaje.
        float currentOrientation = rb.rotation.eulerAngles.y;

        // Diferencia angular más corta entre ambas orientaciones.
        float rotation = Mathf.DeltaAngle(
            currentOrientation,
            targetOrientation
        ) * Mathf.Deg2Rad;

        float rotationSize = Mathf.Abs(rotation);

        float targetRadiusRad = targetRadius * Mathf.Deg2Rad;
        float slowRadiusRad = slowRadius * Mathf.Deg2Rad;

        // Comprobar si ya está orientado correctamente.
        if (rotationSize < targetRadiusRad)
        {
            return steering;
        }

        float targetRotation;

        // Fuera del radio de desaceleración.
        if (rotationSize > slowRadiusRad)
        {
            targetRotation = maxRotation;
        }
        else
        {
            // Reducir progresivamente la velocidad angular.
            targetRotation =
                maxRotation * rotationSize / slowRadiusRad;
        }

        // Determinar el sentido del giro.
        targetRotation *= rotation / rotationSize;

        // Calcular la aceleración angular.
        steering.angular =
            (targetRotation - rb.angularVelocity.y)
            / timeToTarget;

        // Limitar la aceleración angular.
        steering.angular = Mathf.Clamp(
            steering.angular,
            -maxAngularAcceleration,
            maxAngularAcceleration
        );

        steering.linear = Vector3.zero;

        return steering;
    }
}