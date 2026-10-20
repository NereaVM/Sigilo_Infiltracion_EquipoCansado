using UnityEngine;

public class SteeringBehaviourTester : MonoBehaviour
{
    public Transform target;
    public float maxSteeringForce = 10f;
    public float maxLinearVelocity = 5f;

    private SteeringBehaviour movementScript;
    private Rigidbody rb;

    void Start()
    {
        movementScript = GetComponent<SteeringBehaviour>();

        if (movementScript == null)
        {
            Debug.LogError("No SteeringBehaviour found on this GameObject.");
            return;
        }

        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("No Rigidbody found on this GameObject.");
            return;
        }

        movementScript.targetTransform = target;
    }

    void Update()
    {
        if (movementScript == null || rb == null)
            return;

        rb.maxLinearVelocity = maxLinearVelocity;
        movementScript.maxSteeringForce = maxSteeringForce;
        movementScript.rb = rb;
    }
}