using UnityEngine;

/// <summary>
/// Estado de Retorno del Jabato según la propuesta técnica:
/// - Condición: Salir del estado de Investigación o Alerta.
/// - Acción: Movimiento hacia el punto de ruta de patrulla más cercano.
/// - Cono de visión activo.
/// - Salida:
///   1. Llegada al objetivo (Transition_PigletReturnReached).
///   2. Entrada en Alerta si ve al mono (Transition_Vision).
///   3. Entrada en Investigación si oye un sonido (Transition_Hearing).
/// </summary>
public class PigletReturnState : State
{
    [Header("Configuración de Retorno")]
    [SerializeField] private PigletPatrolState patrolState;
    [SerializeField] private float returnSpeed = 1.6f;
    [SerializeField] private float arrivalDistance = 0.6f;

    private EnemyPathAgent pathAgent;
    private MovementSystemController movementController;
    private KinematicMovement kinematicMovement;

    private Vector3 returnTargetPoint;
    private bool hasDestination = false;
    private float previousSpeed;

    public override void OnEntry()
    {
        Debug.Log("[PigletReturnState] Jabato regresando a su zona de patrulla.");

        movementController = GetComponentInParent<MovementSystemController>();
        pathAgent = GetComponentInParent<EnemyPathAgent>();
        kinematicMovement = GetComponentInParent<KinematicMovement>();

        if (kinematicMovement != null)
        {
            previousSpeed = kinematicMovement.speed;
            kinematicMovement.speed = returnSpeed;
        }

        if (patrolState == null)
        {
            patrolState = GetComponentInParent<StateMachine>().GetComponentInChildren<PigletPatrolState>(true);
        }

        hasDestination = false;

        if (patrolState != null)
        {
            returnTargetPoint = patrolState.GetNearestPatrolPoint();
            hasDestination = true;

            if (CanUsePathfinding())
            {
                pathAgent.SetDestination(returnTargetPoint);
            }
            else if (movementController != null)
            {
                movementController.SetNewTarget(returnTargetPoint);
            }
        }
    }

    public override void OnUpdate()
    {
        // El movimiento cinemático o el agente se encargan de guiar al objetivo
    }

    public bool HasReachedPatrolRoute()
    {
        if (!hasDestination) return false;

        Vector3 myPos = transform.parent != null ? transform.parent.position : transform.position;
        Vector3 diff = returnTargetPoint - myPos;
        diff.y = 0f;

        if (CanUsePathfinding())
        {
            return diff.magnitude <= arrivalDistance || pathAgent.HasReachedDestination();
        }

        return diff.magnitude <= arrivalDistance;
    }

    private bool CanUsePathfinding()
    {
        return pathAgent != null &&
               pathAgent.isActiveAndEnabled &&
               pathAgent.gridManager != null &&
               pathAgent.gridManager.Grid != null &&
               pathAgent.pathfinding != null;
    }

    public override void OnExit()
    {
        if (kinematicMovement != null)
        {
            kinematicMovement.speed = previousSpeed;
            kinematicMovement.Stop();
        }

        if (pathAgent != null)
        {
            pathAgent.ClearDestination();
        }

        Debug.Log("[PigletReturnState] Jabato ha regresado con éxito a su patrulla.");
    }
}
