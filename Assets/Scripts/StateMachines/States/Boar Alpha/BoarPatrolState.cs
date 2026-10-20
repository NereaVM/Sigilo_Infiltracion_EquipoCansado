
using UnityEngine;

public class BoarPatrolState : State
{
    [Header("Patrol Area")]
    [SerializeField] private Transform patrolCenter;
    [SerializeField] private Transform patrolTarget;

    [SerializeField] private float patrolRadius = 2f;
    [SerializeField] private float minPatrolDistance = 0.8f;
    [SerializeField] private float reachedDistance = 0.5f;
    [SerializeField] private float wallClearance = 0.45f;

    private MovementSystemController movementController;
    private EnemyPathAgent pathAgent;
    private Steering_Arrive arrive;
    private Steering_Face face;

    private LayerMask obstacleMask;

    private bool patrolReady;
    private bool hasTarget;
    private float nextAttemptTime;

    [Header("Patrol Speed")]
    [SerializeField] private float patrolSpeed = 1.2f;

    private Rigidbody rb;
    private float originalMaxSpeed;

    public override void OnEntry()
    {
        Debug.Log("Boar entered Patrol state.");

        rb = GetComponentInParent<Rigidbody>();

        if (rb != null)
        {
            originalMaxSpeed = rb.maxLinearVelocity;
            rb.maxLinearVelocity = patrolSpeed;
        }

        movementController =
            GetComponentInParent<MovementSystemController>();

        pathAgent = GetComponentInParent<EnemyPathAgent>();

        // Limpiamos cualquier ruta anterior de A*.
        if (pathAgent != null)
            pathAgent.ClearDestination();

        // Buscamos los componentes exactos, evitando
        // confundir Arrive con Pursue o Face con Wander.
        foreach (Steering_Arrive component in
                 GetComponentsInParent<Steering_Arrive>(true))
        {
            if (component.GetType() == typeof(Steering_Arrive))
                arrive = component;
        }

        foreach (Steering_Face component in
                 GetComponentsInParent<Steering_Face>(true))
        {
            if (component.GetType() == typeof(Steering_Face))
                face = component;
        }

        patrolReady =
            movementController != null &&
            pathAgent != null &&
            pathAgent.gridManager != null &&
            arrive != null &&
            face != null &&
            patrolCenter != null &&
            patrolTarget != null;

        if (!patrolReady)
        {
            Debug.LogWarning(
                "Boar Patrol: Missing references. Using original movement."
            );
            return;
        }

        obstacleMask = pathAgent.gridManager.obstacleMask;

        // Usamos el objetivo de patrulla, no el jugador.
        movementController.SetNewTarget(patrolTarget);

        // Movimiento y orientación.
        movementController.ChangeToMovementScripts(
            new MovementScript[] { arrive, face }
        );

        hasTarget = false;
        nextAttemptTime = 0f;

        ChooseNewDestination();
    }

    public override void OnUpdate()
    {
        if (!patrolReady || Time.time < nextAttemptTime)
            return;

        Vector3 difference =
            patrolTarget.position - transform.parent.position;

        difference.y = 0f;

        // Cuando llega, selecciona otro destino.
        if (!hasTarget || difference.magnitude <= reachedDistance)
        {
            ChooseNewDestination();
        }
    }

    public override void OnExit()
    {
        if (rb != null)
        {
            rb.maxLinearVelocity = originalMaxSpeed;
        }

        if (patrolReady)
        {
            // Dejamos de utilizar el objetivo de patrulla.
            movementController.SetNewTarget((Transform)null);

            arrive.targetTransform = null;
            face.targetTransform = null;
        }

        hasTarget = false;
        patrolReady = false;

        Debug.Log("Boar exited Patrol state.");
    }

    private void ChooseNewDestination()
    {
        Vector3 boarPosition = movementController.transform.position;

        // Buscamos un destino dentro de la zona de patrulla.
        for (int attempt = 0; attempt < 20; attempt++)
        {
            Vector2 offset =
                Random.insideUnitCircle * patrolRadius;

            Vector3 destination = patrolCenter.position +
                new Vector3(offset.x, 0f, offset.y);

            destination.y = boarPosition.y;

            Vector3 direction = destination - boarPosition;
            direction.y = 0f;

            float distance = direction.magnitude;

            if (distance < minPatrolDistance)
                continue;

            Vector3 start =
                boarPosition + Vector3.up * 0.5f;

            Vector3 end =
                destination + Vector3.up * 0.5f;

            // Comprobar que el destino tenga espacio.
            if (Physics.CheckSphere(
                end,
                wallClearance,
                obstacleMask,
                QueryTriggerInteraction.Ignore))
            {
                continue;
            }

            // Comprobar que no haya paredes en el trayecto.
            if (Physics.SphereCast(
                start,
                wallClearance,
                direction.normalized,
                out _,
                distance,
                obstacleMask,
                QueryTriggerInteraction.Ignore))
            {
                continue;
            }

            // Destino aceptado.
            patrolTarget.position = destination;

            movementController.SetNewTarget(patrolTarget);

            arrive.targetTransform = patrolTarget;
            face.targetTransform = patrolTarget;

            hasTarget = true;
            nextAttemptTime = Time.time + 0.5f;

            return;
        }

        // Si no encuentra un camino directo seguro,
        // se detiene y vuelve a intentarlo más tarde.
        hasTarget = false;

        movementController.SetNewTarget((Transform)null);
        movementController.SetNewTarget(boarPosition);

        arrive.targetTransform = null;
        face.targetTransform = null;

        nextAttemptTime = Time.time + 1f;
    }
}
