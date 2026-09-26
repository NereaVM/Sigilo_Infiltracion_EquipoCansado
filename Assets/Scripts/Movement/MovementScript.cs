using UnityEngine;

public abstract class MovementScript : MonoBehaviour
{
    public float maxSteeringForce;
    public Vector3 target;
    public Rigidbody rb;
}