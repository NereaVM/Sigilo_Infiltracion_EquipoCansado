using UnityEngine;

public abstract class MovementScript : MonoBehaviour
{
    public Vector3 target;
    public Transform targetTransform;
    public abstract void OnUpdate();
}