using UnityEngine;

public class OwlRelocateState : State
{
    private enum FlightPhase
    {
        Ascending,
        Travelling,
        Descending,
        Finished
    }

    [Header("References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private VisionSense visionSense;
    [SerializeField] private Light visionLight;

    [Header("Waypoints")]
    [SerializeField] private Transform[] flightWaypoints;
    [SerializeField] private bool randomOrder = true;

    [Header("Flight")]
    [SerializeField] private float flightHeightOffset = 10f;
    [SerializeField] private float ascentSpeed = 4f;
    [SerializeField] private float travelSpeed = 5f;
    [SerializeField] private float descentSpeed = 4f;
    [SerializeField] private float arrivalDistance = 0.15f;

    public bool HasFinished { get; private set; }

    private FlightPhase currentPhase;

    private Vector3 ascentTarget;
    private Vector3 travelTarget;
    private Vector3 landingTarget;

    private bool previousUseGravity;
    private bool previousLightEnabled;
    private bool previousVisionEnabled;

    public override void OnEntry()
    {
        HasFinished = false;

        if (rb == null || flightWaypoints == null || flightWaypoints.Length == 0)
        {
            HasFinished = true;
            return;
        }

        Transform selectedWaypoint = SelectNextWaypoint();

        if (selectedWaypoint == null)
        {
            HasFinished = true;
            return;
        }

        previousUseGravity = rb.useGravity;
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;

        if (visionSense != null)
        {
            previousVisionEnabled = visionSense.enabled;
            visionSense.SetVisionEnabled(false);
        }

        if (visionLight != null)
        {
            previousLightEnabled = visionLight.enabled;
            visionLight.enabled = false;
        }

        landingTarget = selectedWaypoint.position;

        float flightY =
            Mathf.Max(rb.position.y, landingTarget.y) +
            flightHeightOffset;

        ascentTarget = new Vector3(
            rb.position.x,
            flightY,
            rb.position.z
        );

        travelTarget = new Vector3(
            landingTarget.x,
            flightY,
            landingTarget.z
        );

        currentPhase = FlightPhase.Ascending;
    }

    public override void OnUpdate()
    {
        if (HasFinished || rb == null)
            return;

        switch (currentPhase)
        {
            case FlightPhase.Ascending:
                if (MoveTowards(ascentTarget, ascentSpeed))
                    currentPhase = FlightPhase.Travelling;
                break;

            case FlightPhase.Travelling:
                if (MoveTowards(travelTarget, travelSpeed))
                    currentPhase = FlightPhase.Descending;
                break;

            case FlightPhase.Descending:
                if (MoveTowards(landingTarget, descentSpeed))
                {
                    rb.position = landingTarget;
                    rb.linearVelocity = Vector3.zero;

                    currentPhase = FlightPhase.Finished;
                    HasFinished = true;
                }
                break;

            case FlightPhase.Finished:
                rb.linearVelocity = Vector3.zero;
                break;
        }
    }

    public override void OnExit()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.useGravity = previousUseGravity;
        }

        if (visionSense != null)
            visionSense.SetVisionEnabled(previousVisionEnabled);

        if (visionLight != null)
            visionLight.enabled = previousLightEnabled;
    }

    private bool MoveTowards(Vector3 targetPosition, float speed)
    {
        Vector3 direction = targetPosition - rb.position;
        float distance = direction.magnitude;

        if (distance <= arrivalDistance)
        {
            rb.position = targetPosition;
            rb.linearVelocity = Vector3.zero;
            return true;
        }

        rb.linearVelocity =
            direction.normalized * Mathf.Max(0f, speed);

        return false;
    }

    private Transform SelectNextWaypoint()
    {
        if (flightWaypoints.Length == 1)
            return flightWaypoints[0];

        int closestIndex = FindClosestWaypointIndex();

        if (randomOrder)
        {
            int selectedIndex = closestIndex;

            for (int attempts = 0; attempts < 20; attempts++)
            {
                int candidate =
                    Random.Range(0, flightWaypoints.Length);

                if (candidate != closestIndex &&
                    flightWaypoints[candidate] != null)
                {
                    selectedIndex = candidate;
                    break;
                }
            }

            return flightWaypoints[selectedIndex];
        }

        for (int offset = 1; offset <= flightWaypoints.Length; offset++)
        {
            int index =
                (closestIndex + offset) % flightWaypoints.Length;

            if (flightWaypoints[index] != null)
                return flightWaypoints[index];
        }

        return null;
    }

    private int FindClosestWaypointIndex()
    {
        int closestIndex = 0;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < flightWaypoints.Length; i++)
        {
            if (flightWaypoints[i] == null)
                continue;

            float distance = Vector3.SqrMagnitude(
                rb.position - flightWaypoints[i].position
            );

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }

        return closestIndex;
    }
}
