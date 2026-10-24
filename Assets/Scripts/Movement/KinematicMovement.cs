
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class KinematicMovement : MovementScript
{
    [Header("Cinematic Settings")]
    public float speed = 1.6f;
    public float rotationSpeed = 300f;
    public float arrivalRadius = 0.35f;
    public bool stopOnArrival = true;

    [Header("Animal Avoidance")]
    [SerializeField] private float pigletAvoidanceRadius = 1.2f;
    [SerializeField] private float boarAvoidanceRadius = 1.8f;
    [SerializeField] private float avoidanceStrength = 2f;

    private readonly Collider[] nearbyColliders = new Collider[32];
    private readonly HashSet<Rigidbody> checkedAnimals = new HashSet<Rigidbody>();

    private Rigidbody rb;
    private float groundY;
    private bool hasGroundY = false;
    private bool movementPaused = false;

    public void PauseMovement()
    {
        movementPaused = true;
        Stop();
    }

    public void ResumeMovement()
    {
        movementPaused = false;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        targetTransform = null;
    }

    private void Start()
    {
        groundY = transform.position.y;
        hasGroundY = true;
    }

    public override void OnUpdate()
    {
        if (movementPaused)
        {
            Stop();
            return;
        }

        if (targetTransform != null)
        {
            target = targetTransform.position;
        }

        Vector3 diff = target - transform.position;
        diff.y = 0f;

        float distance = diff.magnitude;

        if (distance > arrivalRadius)
        {
            Vector3 desiredDirection = diff.normalized;

            // Evitar animales cercanos.
            Vector3 avoidance = CalculateAvoidance(desiredDirection);

            Vector3 finalDirection =
                desiredDirection + avoidance * avoidanceStrength;

            if (finalDirection.sqrMagnitude > 0.001f)
            {
                finalDirection.Normalize();

                rb.linearVelocity = new Vector3(
                    finalDirection.x * speed,
                    0f,
                    finalDirection.z * speed
                );

                rb.angularVelocity = Vector3.zero;

                Quaternion targetRot = Quaternion.LookRotation(
                    finalDirection,
                    Vector3.up
                );

                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRot,
                    rotationSpeed * Time.deltaTime
                );
            }
            else
            {
                Stop();
            }
        }
        else if (stopOnArrival)
        {
            Stop();
        }

        if (hasGroundY &&
            Mathf.Abs(transform.position.y - groundY) > 0.05f)
        {
            transform.position = new Vector3(
                transform.position.x,
                groundY,
                transform.position.z
            );
        }
    }

    private Vector3 CalculateAvoidance(Vector3 desiredDirection)
    {
        Vector3 avoidance = Vector3.zero;
        checkedAnimals.Clear();

        float searchRadius = Mathf.Max(
            pigletAvoidanceRadius,
            boarAvoidanceRadius
        );

        int count = Physics.OverlapSphereNonAlloc(
            rb.position,
            searchRadius,
            nearbyColliders,
            ~0,
            QueryTriggerInteraction.Ignore
        );

        for (int i = 0; i < count; i++)
        {
            Collider other = nearbyColliders[i];

            if (other == null)
                continue;

            Rigidbody otherRb = other.attachedRigidbody;

            if (otherRb == null || otherRb == rb)
                continue;

            if (!checkedAnimals.Add(otherRb))
                continue;

            bool isBoar =
                otherRb.GetComponent<HearingSense_Boar>() != null;

            bool isPiglet =
                otherRb.GetComponent<HearingSense_Piglet>() != null;

            if (!isBoar && !isPiglet)
                continue;

            float radius = isBoar
                ? boarAvoidanceRadius
                : pigletAvoidanceRadius;

            Vector3 away = rb.position - otherRb.position;
            away.y = 0f;

            float distance = away.magnitude;

            if (distance >= radius)
                continue;

            if (distance < 0.001f)
            {
                away = Vector3.Cross(
                    Vector3.up,
                    desiredDirection
                );

                if (away.sqrMagnitude < 0.001f)
                    away = Vector3.right;

                distance = 0.001f;
            }

            away.Normalize();

            float influence = 1f - distance / radius;

            // Separación para mantener distancia.
            avoidance += away * influence;

            // Favorece un desvío lateral si el animal
            // se encuentra delante de nuestro movimiento.
            float inFront = Vector3.Dot(
                desiredDirection,
                -away
            );

            if (inFront > 0.5f)
            {
                Vector3 lateral = Vector3.Cross(
                    Vector3.up,
                    desiredDirection
                );

                avoidance += lateral * influence * 0.7f;
            }
        }

        return avoidance;
    }

    public void Stop()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(
            transform.position,
            pigletAvoidanceRadius
        );

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(
            transform.position,
            boarAvoidanceRadius
        );
    }
}
