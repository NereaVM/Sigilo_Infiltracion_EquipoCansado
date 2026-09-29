using UnityEngine;

public abstract class MovementScript : MonoBehaviour
{
    public float maxSteeringForce;
    public Transform targetTransform;
    public Vector3 target;
    public Rigidbody rb;

}