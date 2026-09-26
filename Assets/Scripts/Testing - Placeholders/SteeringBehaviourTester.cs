using UnityEngine;

public class SteeringBehaviourTester : MonoBehaviour
{
    public Transform target;
    public float maxSteeringForce = 10f;
    public float maxLinearVelocity = 5f;
    private MovementScript movementScript;
    private Rigidbody rb;

    void Start()
    {
        movementScript = GetComponent<MovementScript>();
        if (movementScript == null)
        {
            Debug.LogError("No MovementScript found on this GameObject.");
        }
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("No Rigidbody found on this GameObject.");
        }
    }

    void Update()
    {
        rb.maxLinearVelocity = maxLinearVelocity;
        movementScript.maxSteeringForce = maxSteeringForce;
        movementScript.target = target.position;
    }
}