using UnityEngine;


public class Grabbable : MonoBehaviour
{
    private Rigidbody rb;
    private Collider col;

    public bool IsBeingHeld { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    
    public void PickUp(Transform holdPoint)
    {
        IsBeingHeld = true;

        //physics off
        rb.isKinematic = true;
        rb.useGravity = false;

        //disable the collider to prevent it from pushing the player
        if (col != null)
            col.enabled = false;
     
        transform.SetParent(holdPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }


    public void Throw(Vector3 force)
    {
        IsBeingHeld = false;

        //detach from the father
        transform.SetParent(null);

        //physics on
        if (col != null)
            col.enabled = true;

        rb.isKinematic = false;
        rb.useGravity = true;

        //clean inertias
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        
        rb.AddForce(force, ForceMode.Impulse);
    }
}
