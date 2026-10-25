using UnityEngine;

public class CrocodileIdleState : State
{
    /* 
        In the idle state, crocodile slowly pans its sight
        from one side to the other in search for targets.
    */

    private Rigidbody rb;
    [SerializeField] private float panSpeed = 1f;
    [SerializeField] private float panAngle = 45f; // Full ROM is angle * 2, angle assumes the crocodile starts by facing forward (the middle of the pan)

    private Quaternion initialRotation;
    private float panPosition;
    private float panDirection;

    private void Start()
    {
        rb = GetComponentInParent<Rigidbody>();
        // We set up the look direction of the crocodile in the scene
        initialRotation = rb.rotation;
    }

    public override void OnEntry()
    {
        rb = GetComponentInParent<Rigidbody>();

        panPosition = 0f;
        panDirection = 1f;
    }

    public override void OnExit(){}

    public override void OnUpdate()
    {
        if (rb == null)
        {
            return;
        }

        float maximumAngle = Mathf.Max(0f, panAngle);
        if (maximumAngle == 0f || panSpeed == 0f)
        {
            rb.MoveRotation(initialRotation);
            return;
        }

        panPosition += Mathf.Abs(panSpeed) * panDirection * Time.deltaTime;

        if (panPosition >= maximumAngle)
        {
            panPosition = maximumAngle;
            panDirection = -1f;
        }
        else if (panPosition <= -maximumAngle)
        {
            panPosition = -maximumAngle;
            panDirection = 1f;
        }

        rb.MoveRotation(
            initialRotation * Quaternion.Euler(0f, panPosition, 0f)
        );
    }

}