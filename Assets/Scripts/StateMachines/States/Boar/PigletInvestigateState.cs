using UnityEngine;

/// <summary>
/// Estado de Investigación del Jabato según la propuesta técnica:
/// - Condición: Entra en el rango de un sonido activo.
/// - Acción: Movimiento hacia el origen del sonido.
/// - Cono de visión activo.
/// - Salida:
///   1. Se aleja demasiado del punto de patrulla más cercano.
///   2. Transcurre un tiempo sin nuevos estímulos tras llegar.
///   3. Ve al jugador -> pasa a Alerta (Transition_Vision).
/// </summary>
public class PigletInvestigateState : State
{
    [Header("Configuración de Investigación")]
    [Tooltip("Velocidad cinemática durante la investigación.")]
    [SerializeField] private float investigateSpeed = 1.6f;

    [Tooltip("Tiempo de espera observando el punto tras llegar antes de volver.")]
    [SerializeField] private float waitAtPointDuration = 3.0f;

    [Tooltip("Distancia máxima permitida alejándose de la patrulla antes de cancelar la investigación.")]
    [SerializeField] private float maxDistanceFromPatrol = 12.0f;

    [Header("Referencias Opcionales")]
    [SerializeField] private PigletPatrolState patrolState;

    private EnemyPathAgent pathAgent;
    private MovementSystemController movementController;
    private KinematicMovement kinematicMovement;
    private HearingSense hearingSense;

    private Vector3 originPatrolPoint;
    private float timerAtPoint = 0f;
    private bool hasReachedPoint = false;
    private bool shouldReturn = false;
    private float previousSpeed;

    public override void OnEntry()
    {
        Debug.Log("[PigletInvestigateState] Jabato investigando origen del sonido.");

        movementController = GetComponentInParent<MovementSystemController>();
        pathAgent = GetComponentInParent<EnemyPathAgent>();
        kinematicMovement = GetComponentInParent<KinematicMovement>();
        hearingSense = GetComponentInParent<HearingSense>();

        if (patrolState == null)
        {
            patrolState = GetComponentInParent<StateMachine>().GetComponentInChildren<PigletPatrolState>(true);
        }

        if (patrolState != null)
        {
            originPatrolPoint = patrolState.GetNearestPatrolPoint();
        }
        else
        {
            originPatrolPoint = transform.parent != null ? transform.parent.position : transform.position;
        }

        if (kinematicMovement != null)
        {
            previousSpeed = kinematicMovement.speed;
            kinematicMovement.speed = investigateSpeed;
        }

        hasReachedPoint = false;
        timerAtPoint = 0f;
        shouldReturn = false;

        // Moverse al origen del sonido
        if (hearingSense != null && hearingSense.hasHeardSound)
        {
            Vector3 soundPos = hearingSense.lastSoundPosition;

            if (pathAgent != null && pathAgent.isActiveAndEnabled)
            {
                pathAgent.SetDestination(soundPos);
            }
            else if (movementController != null)
            {
                movementController.SetNewTarget(soundPos);
            }
        }
    }

    public override void OnUpdate()
    {
        if (shouldReturn) return;

        Vector3 currentPos = transform.parent != null ? transform.parent.position : transform.position;

        // 1. Condición de salida del documento: ¿Se ha alejado demasiado del punto de patrulla?
        float distFromPatrol = Vector3.Distance(currentPos, originPatrolPoint);
        if (distFromPatrol > maxDistanceFromPatrol)
        {
            Debug.Log("[PigletInvestigateState] Demasiado lejos de la patrulla. Volviendo.");
            shouldReturn = true;
            return;
        }

        // 2. Comprobar si ha llegado al punto del sonido
        if (!hasReachedPoint)
        {
            bool reached = false;
            if (pathAgent != null && pathAgent.isActiveAndEnabled)
            {
                reached = pathAgent.HasReachedDestination();
            }
            else if (hearingSense != null)
            {
                Vector3 diff = hearingSense.lastSoundPosition - currentPos;
                diff.y = 0f;
                reached = diff.magnitude <= 0.6f;
            }

            if (reached)
            {
                hasReachedPoint = true;
                timerAtPoint = 0f;
                if (pathAgent != null) pathAgent.ClearDestination();
                if (kinematicMovement != null) kinematicMovement.Stop();
            }
        }
        else
        {
            // 3. Condición de salida: Esperar unos segundos mirando
            timerAtPoint += Time.deltaTime;
            if (timerAtPoint >= waitAtPointDuration)
            {
                shouldReturn = true;
            }
        }
    }

    public bool ShouldReturn()
    {
        return shouldReturn;
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

        Debug.Log("[PigletInvestigateState] Fin de investigación.");
    }
}
