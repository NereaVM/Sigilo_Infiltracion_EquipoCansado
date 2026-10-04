
using UnityEngine;

public class Steering_Face : Steering_Align
{
    public override SteeringOutput GetSteering()
    {
        if (targetTransform == null)
        {
            return new SteeringOutput();
        }

        return FacePosition(targetTransform.position);
    }

    // Permite que Wander reutilice Face con un objetivo calculado.
    protected SteeringOutput FacePosition(Vector3 targetPosition)
    {
        // Dirección desde el jabalí hacia el objetivo.
        Vector3 direction =
            targetPosition - rb.position;

        direction.y = 0f;

        // Evitar calcular una orientación indefinida.
        if (direction.sqrMagnitude < 0.0001f)
        {
            return new SteeringOutput();
        }

        // Calcular la orientación hacia el objetivo.
        targetOrientation =
            Mathf.Atan2(direction.x, direction.z)
            * Mathf.Rad2Deg;

        // Delegar el cálculo de la aceleración en Align.
        return base.GetSteering();
    }
}
