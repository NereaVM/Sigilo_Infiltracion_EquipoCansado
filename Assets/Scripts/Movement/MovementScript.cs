using UnityEngine;

public abstract class MovementScript : MonoBehaviour
{
    public Vector3 target;
    public abstract void OnUpdate();
}