
using UnityEngine;

/// <summary>
/// Estado de Patrulla del Jabato según la propuesta técnica:
/// - Patrulla definida: Jabato mediano, circuito de waypoints.
/// - Patrulla libre: Jabato pequeño, destinos aleatorios.
/// - Movimiento Cinemático Simple.
/// - Cono de visión activo. Oído activo.
/// </summary>
public class PigletPatrolState : State
{
    public enum PatrolMode
    {
        DefinedWaypoints,
        FreeRandom
    }

    [Header("Tipo de Jabato")]
    [SerializeField] private PatrolMode patrolMode = PatrolMode.FreeRandom;

    [Header("Patrulla Definida (Jabato Mediano)")]
    [Tooltip("Lista de puntos del circuito predefinido.")]
    [SerializeField] private Transform[] waypoints;

    [SerializeField] private float waypointReachedThreshold = 0.5f;
    [SerializeField] private float waitAtWaypointDuration = 1.0f;

    [Header("Patrulla Libre (Jabato Pequeño)")]
    [Tooltip("Centro de la zona de patrulla libre. Si no se asigna, usa el punto inicial.")]
    [SerializeField] private Transform patrolCenter;

    [SerializeField] private float patrolRadius = 4f;
    [SerializeField] private float minPatrolDistance = 1.5f;
    [SerializeField] private float wallClearance = 0.65f;

    [Header("Velocidad Cinemática")]
    [SerializeField] private float patrolSpeed = 1.4f;

    private EnemyPathAgent pathAgent;
    private MovementSystemController movementController;
    private KinematicMovement kinematicMovement;

    private int currentWaypointIndex = 0;
    private float waitTimer = 0f;
    private bool isWaiting = false;

    private Vector3 currentTargetPosition;
    private float previousSpeed;

    private Vector3 spawnPosition;
    private bool hasSpawnPosition = false;

    // Sistema antiatascos.
    private float stuckTimer = 0f;
    private Vector3 lastCheckedPosition;
    private float checkIntervalTimer = 0f;

    public override void OnEntry()
    {
        Debug.Log(
            $"[PigletPatrolState] Jabato patrullando en modo: {patrolMode}."
        );

        movementController =
            GetComponentInParent<MovementSystemController>();

        pathAgent =
            GetComponentInParent<EnemyPathAgent>();

        kinematicMovement =
            GetComponentInParent<KinematicMovement>();

        Transform jabatoRoot =
            transform.parent != null ? transform.parent : transform;

        if (!hasSpawnPosition)
        {
            spawnPosition = jabatoRoot.position;
            hasSpawnPosition = true;
        }

        // Configurar velocidad y reactivar movimiento.
        if (kinematicMovement != null)
        {
            previousSpeed = kinematicMovement.speed;
            kinematicMovement.speed = patrolSpeed;
            kinematicMovement.targetTransform = null;

            kinematicMovement.ResumeMovement();
        }

        if (movementController != null)
        {
            movementController.currentTargetTransform = null;
        }

        if (pathAgent != null)
        {
            pathAgent.ClearDestination();
        }

        // Reiniciar temporizadores.
        isWaiting = false;
        waitTimer = 0f;
        stuckTimer = 0f;
        checkIntervalTimer = 0f;
        lastCheckedPosition = jabatoRoot.position;

        // Elegir comportamiento según el tipo de jabato.
        if (patrolMode == PatrolMode.DefinedWaypoints)
        {
            MoveToCurrentWaypoint();
        }
        else
        {
            ChooseRandomDestination();
        }
    }

    public override void OnUpdate()
    {
        // Comprobar periódicamente si el jabato está atascado.
        checkIntervalTimer += Time.deltaTime;

        if (checkIntervalTimer >= 1.0f)
        {
            checkIntervalTimer = 0f;

            Vector3 currentPos =
                transform.parent != null
                    ? transform.parent.position
                    : transform.position;

            if (Vector3.Distance(
                    currentPos,
                    lastCheckedPosition
                ) < 0.15f && !isWaiting)
            {
                stuckTimer += 1.0f;

                if (stuckTimer >= 2.5f)
                {
                    stuckTimer = 0f;

                    if (pathAgent != null)
                    {
                        pathAgent.ClearDestination();
                    }

                    if (kinematicMovement != null)
                    {
                        kinematicMovement.Stop();
                    }

                    // CORRECCIÓN:
                    // Cada tipo de jabato resuelve
                    // el atasco de forma diferente.

                    if (patrolMode == PatrolMode.FreeRandom)
                    {
                        // Jabato pequeño:
                        // Buscar otro destino aleatorio.
                        Debug.Log(
                            "[PigletPatrolState] Jabato libre atascado. Buscando otro destino."
                        );

                        ChooseRandomDestination();
                    }
                    else if (waypoints != null &&
                             waypoints.Length > 0)
                    {
                        // Jabato mediano:
                        // Continuar con el siguiente waypoint.
                        Debug.Log(
                            "[PigletPatrolState] Jabato de ruta atascado. Siguiente waypoint."
                        );

                        currentWaypointIndex =
                            (currentWaypointIndex + 1) %
                            waypoints.Length;

                        MoveToCurrentWaypoint();
                    }

                    lastCheckedPosition = currentPos;
                    return;
                }
            }
            else
            {
                stuckTimer = 0f;
            }

            lastCheckedPosition = currentPos;
        }

        if (patrolMode == PatrolMode.DefinedWaypoints)
        {
            UpdateDefinedWaypoints();
        }
        else
        {
            UpdateFreeRandom();
        }
    }

    // ==========================================
    // PATRULLA DEFINIDA - JABATO MEDIANO
    // ==========================================

    private void UpdateDefinedWaypoints()
    {
        if (waypoints == null || waypoints.Length == 0)
            return;

        Transform targetWp = waypoints[currentWaypointIndex];

        if (targetWp == null)
            return;

        // Esperar detenido antes de continuar.
        if (isWaiting)
        {
            waitTimer += Time.deltaTime;

            if (waitTimer >= waitAtWaypointDuration)
            {
                isWaiting = false;
                waitTimer = 0f;

                currentWaypointIndex =
                    (currentWaypointIndex + 1) %
                    waypoints.Length;

                MoveToCurrentWaypoint();
            }

            return;
        }

        Vector3 myPos =
            transform.parent != null
                ? transform.parent.position
                : transform.position;

        Vector3 diff = targetWp.position - myPos;
        diff.y = 0f;

        bool reached;

        if (pathAgent != null && pathAgent.isActiveAndEnabled)
        {
            reached =
                diff.magnitude <= waypointReachedThreshold ||
                pathAgent.HasReachedDestination();
        }
        else
        {
            reached = diff.magnitude <= waypointReachedThreshold;
        }

        if (reached)
        {
            isWaiting = true;
            waitTimer = 0f;

            if (pathAgent != null)
            {
                pathAgent.ClearDestination();
            }

            // CORRECCIÓN:
            // Pausa real mientras espera.
            if (kinematicMovement != null)
            {
                kinematicMovement.PauseMovement();
            }
        }
    }

    private void MoveToCurrentWaypoint()
    {
        if (waypoints == null || waypoints.Length == 0)
            return;

        Transform targetWp = waypoints[currentWaypointIndex];

        if (targetWp == null)
            return;

        currentTargetPosition = targetWp.position;

        if (movementController != null)
        {
            movementController.currentTargetTransform = null;
        }

        if (kinematicMovement != null)
        {
            kinematicMovement.targetTransform = null;
        }

        if (pathAgent != null && pathAgent.isActiveAndEnabled)
        {
            pathAgent.SetDestination(currentTargetPosition);
        }
        else if (movementController != null)
        {
            movementController.SetNewTarget(currentTargetPosition);
        }

        // CORRECCIÓN:
        // Reanudar al tener un nuevo destino.
        if (kinematicMovement != null)
        {
            kinematicMovement.ResumeMovement();
        }
    }

    // ==========================================
    // PATRULLA LIBRE - JABATO PEQUEÑO
    // ==========================================

    private void UpdateFreeRandom()
    {
        Transform jabatoRoot =
            transform.parent != null
                ? transform.parent
                : transform;

        Vector3 myPos = jabatoRoot.position;

        if (isWaiting)
        {
            // Mientras espera, comprueba si tiene
            // una pared delante y mira hacia una zona abierta.

            LayerMask obstacleMask =
                (pathAgent != null &&
                 pathAgent.gridManager != null)
                    ? pathAgent.gridManager.obstacleMask
                    : (LayerMask)Physics.DefaultRaycastLayers;

            if (Physics.Raycast(
                    myPos + Vector3.up * 0.35f,
                    jabatoRoot.forward,
                    1.4f,
                    obstacleMask,
                    QueryTriggerInteraction.Ignore))
            {
                Vector3 center =
                    patrolCenter != null
                        ? patrolCenter.position
                        : spawnPosition;

                Vector3 lookDir = center - myPos;
                lookDir.y = 0f;

                if (lookDir.sqrMagnitude > 0.01f)
                {
                    Quaternion targetRot =
                        Quaternion.LookRotation(
                            lookDir.normalized,
                            Vector3.up
                        );

                    jabatoRoot.rotation =
                        Quaternion.RotateTowards(
                            jabatoRoot.rotation,
                            targetRot,
                            150f * Time.deltaTime
                        );
                }
            }

            waitTimer += Time.deltaTime;

            if (waitTimer >= waitAtWaypointDuration)
            {
                isWaiting = false;
                waitTimer = 0f;

                ChooseRandomDestination();
            }

            return;
        }

        Vector3 diff = currentTargetPosition - myPos;
        diff.y = 0f;

        bool reached;

        if (pathAgent != null && pathAgent.isActiveAndEnabled)
        {
            reached = pathAgent.HasReachedDestination();
        }
        else
        {
            reached = diff.magnitude <= waypointReachedThreshold;
        }

        if (reached)
        {
            isWaiting = true;
            waitTimer = 0f;

            if (pathAgent != null)
            {
                pathAgent.ClearDestination();
            }

            // CORRECCIÓN:
            // El jabato permanece completamente detenido.
            if (kinematicMovement != null)
            {
                kinematicMovement.PauseMovement();
            }
        }
    }

    private void ChooseRandomDestination()
    {
        Transform jabatoRoot =
            transform.parent != null
                ? transform.parent
                : transform;

        Vector3 myPos = jabatoRoot.position;

        Vector3 center =
            patrolCenter != null
                ? patrolCenter.position
                : spawnPosition;

        LayerMask obstacleMask = Physics.DefaultRaycastLayers;

        if (pathAgent != null && pathAgent.gridManager != null)
        {
            obstacleMask = pathAgent.gridManager.obstacleMask;
        }

        // Buscar un destino seguro.
        for (int i = 0; i < 35; i++)
        {
            Vector2 circle =
                Random.insideUnitCircle * patrolRadius;

            Vector3 candidate =
                center + new Vector3(circle.x, 0f, circle.y);

            candidate.y = myPos.y;

            Vector3 diff = candidate - myPos;
            diff.y = 0f;

            if (diff.magnitude < minPatrolDistance)
                continue;

            // 1. Comprobar nodo transitable en A*.
            if (pathAgent != null &&
                pathAgent.gridManager != null)
            {
                GridNode node =
                    pathAgent.gridManager
                        .GetNodeFromWorldPosition(candidate);

                if (node == null || !node.isWalkable)
                    continue;
            }

            // 2. Evitar destinos pegados a paredes.
            Vector3 checkPoint =
                candidate + Vector3.up * 0.4f;

            if (Physics.CheckSphere(
                    checkPoint,
                    wallClearance,
                    obstacleMask,
                    QueryTriggerInteraction.Ignore))
            {
                continue;
            }

            // 3. Evitar paredes delante del destino.
            Vector3 travelDir = diff.normalized;

            if (Physics.Raycast(
                    checkPoint,
                    travelDir,
                    1.0f,
                    obstacleMask,
                    QueryTriggerInteraction.Ignore))
            {
                continue;
            }

            // 4. Comprobar que la salida esté libre.
            if (Physics.SphereCast(
                    myPos + Vector3.up * 0.4f,
                    0.35f,
                    travelDir,
                    out _,
                    0.8f,
                    obstacleMask,
                    QueryTriggerInteraction.Ignore))
            {
                continue;
            }

            // Destino válido.
            currentTargetPosition = candidate;

            if (movementController != null)
            {
                movementController.currentTargetTransform = null;
            }

            if (kinematicMovement != null)
            {
                kinematicMovement.targetTransform = null;
            }

            if (pathAgent != null && pathAgent.isActiveAndEnabled)
            {
                pathAgent.SetDestination(currentTargetPosition);
            }
            else if (movementController != null)
            {
                movementController.SetNewTarget(currentTargetPosition);
            }

            // CORRECCIÓN:
            // Reanudar movimiento hacia el nuevo punto.
            if (kinematicMovement != null)
            {
                kinematicMovement.ResumeMovement();
            }

            return;
        }

        // Si no encuentra un punto válido,
        // espera antes de volver a intentarlo.
        isWaiting = true;
        waitTimer = 0f;

        if (kinematicMovement != null)
        {
            kinematicMovement.PauseMovement();
        }
    }

    // ==========================================
    // SALIDA DEL ESTADO
    // ==========================================

    public override void OnExit()
    {
        if (kinematicMovement != null)
        {
            kinematicMovement.speed = previousSpeed;
            kinematicMovement.Stop();

            // Evitar que la pausa se mantenga
            // en los siguientes estados.
            kinematicMovement.ResumeMovement();
        }

        if (pathAgent != null)
        {
            pathAgent.ClearDestination();
        }

        isWaiting = false;

        Debug.Log("[PigletPatrolState] Salida de Patrulla.");
    }

    // ==========================================
    // PUNTO DE PATRULLA MÁS CERCANO
    // ==========================================

    public Vector3 GetNearestPatrolPoint()
    {
        Vector3 myPos =
            transform.parent != null
                ? transform.parent.position
                : transform.position;

        if (patrolMode == PatrolMode.DefinedWaypoints &&
            waypoints != null &&
            waypoints.Length > 0)
        {
            Vector3 nearest = myPos;
            float minDist = float.MaxValue;

            foreach (Transform wp in waypoints)
            {
                if (wp == null)
                    continue;

                float d = Vector3.Distance(myPos, wp.position);

                if (d < minDist)
                {
                    minDist = d;
                    nearest = wp.position;
                }
            }

            return nearest;
        }

        return patrolCenter != null
            ? patrolCenter.position
            : spawnPosition;
    }
}
