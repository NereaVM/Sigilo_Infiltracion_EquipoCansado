
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BoarSteeringController : MonoBehaviour
{
    public enum BoarMode
    {
        Patrol,
        Order,
        Pursuit,
        Return
    }

    [Header("Current Mode")]
    [SerializeField] private BoarMode currentMode = BoarMode.Patrol;

    [Header("Steering Behaviours")]
    [SerializeField] private Steering_Wander wander;
    [SerializeField] private Steering_Pursue pursue;
    [SerializeField] private Steering_Arrive arrive;
    [SerializeField] private Steering_Face face;

    [Header("Targets")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform boarDenTarget;
    [SerializeField] private Transform orderTarget;

    [Header("Movement Speeds")]
    [SerializeField] private float pursuitSpeed = 3f;
    [SerializeField] private float returnSpeed = 2.5f;

    private Rigidbody rb;
    private BoarMode previousMode;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Configure the targets before the behaviours start.
        if (pursue != null && player != null)
        {
            pursue.targetTransform = player;
            pursue.target = player.position;
        }

        if (arrive != null && boarDenTarget != null)
        {
            arrive.targetTransform = boarDenTarget;
            arrive.target = boarDenTarget.position;
        }
    }

    private void Start()
    {
        ApplyMode();
        previousMode = currentMode;
    }

    private void Update()
    {
        // Allows changing the mode from the Inspector.
        if (currentMode != previousMode)
        {
            ApplyMode();
            previousMode = currentMode;
        }
    }

    public void SetMode(BoarMode newMode)
    {
        if (currentMode == newMode)
            return;

        currentMode = newMode;

        ApplyMode();
        previousMode = currentMode;
    }

    private void ApplyMode()
    {
        // Disable all behaviours before selecting a new one.
        wander.enabled = false;
        pursue.enabled = false;
        arrive.enabled = false;
        face.enabled = false;

        switch (currentMode)
        {
            case BoarMode.Patrol:

                rb.maxLinearVelocity = wander.maxWanderSpeed;

                wander.enabled = true;

                break;

            case BoarMode.Order:

                // Stop the boar while it gives an order.
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;

                if (orderTarget != null)
                {
                    face.targetTransform = orderTarget;
                    face.enabled = true;
                }

                break;

            case BoarMode.Pursuit:

                rb.maxLinearVelocity = pursuitSpeed;

                pursue.targetTransform = player;
                pursue.target = player.position;

                face.targetTransform = player;

                pursue.enabled = true;
                face.enabled = true;

                break;

            case BoarMode.Return:

                rb.maxLinearVelocity = returnSpeed;

                arrive.targetTransform = boarDenTarget;
                arrive.target = boarDenTarget.position;

                face.targetTransform = boarDenTarget;

                arrive.enabled = true;
                face.enabled = true;

                break;
        }
    }
}
