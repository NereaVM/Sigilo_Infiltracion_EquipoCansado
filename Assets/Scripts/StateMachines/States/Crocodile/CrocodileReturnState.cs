using UnityEngine;

public class CrocodileReturnState : State
{
    [SerializeField] private float arrivalDistance = 0.6f;

    private EnemyPathAgent pathAgent;
    private MovementSystemController movementController;
    private Rigidbody rb;
    private Vector3 returnPosition;
    private bool hasReturnPosition;
    private bool pathFailureReported;

    private void Start()
    {
        rb = GetComponentInParent<Rigidbody>();
        StateMachine stateMachine =
            GetComponentInParent<StateMachine>();

        returnPosition = rb != null
            ? rb.position
            : stateMachine != null
                ? stateMachine.transform.position
                : transform.position;
        hasReturnPosition = true;
    }

    public override void OnEntry()
    {
        pathAgent = GetComponentInParent<EnemyPathAgent>();
        movementController =
            GetComponentInParent<MovementSystemController>();
        pathFailureReported = false;

        if (!hasReturnPosition)
        {
            Debug.LogError(
                "Crocodile Return: Initial position was not captured."
            );
            return;
        }

        if (pathAgent == null ||
            !pathAgent.isActiveAndEnabled ||
            pathAgent.gridManager == null ||
            pathAgent.pathfinding == null)
        {
            Debug.LogError(
                "Crocodile Return: A configured EnemyPathAgent with " +
                "GridManager and AStarPathfinding is required."
            );
            return;
        }

        if (movementController == null)
        {
            Debug.LogError(
                "Crocodile Return: MovementSystemController was not found."
            );
            return;
        }

        movementController.ChangeToMovementScripts(movementScripts);
        movementController.currentTargetTransform = null;

        foreach (MovementScript movementScript in movementScripts)
        {
            if (movementScript == null)
            {
                continue;
            }

            movementScript.targetTransform = null;

            if (movementScript is Steering_Face face)
            {
                face.targetTransform = pathAgent.PathLookTarget;
            }
        }

        pathAgent.SetDestination(returnPosition);
    }

    public override void OnUpdate()
    {
        if (pathAgent != null &&
            pathAgent.PathCalculationFailed &&
            !pathFailureReported)
        {
            pathFailureReported = true;
            Debug.LogError(
                "Crocodile Return: A* could not find a path to the " +
                "saved spawn position."
            );
        }
    }

    public override void OnExit()
    {
        StopMovement();

        if (pathAgent != null)
        {
            pathAgent.ClearDestination();
        }

        foreach (MovementScript movementScript in movementScripts)
        {
            if (movementScript is Steering_Face face)
            {
                face.targetTransform = null;
            }
        }
    }

    private void StopMovement()
    {
        if (movementController != null)
        {
            movementController.ChangeToMovementScripts(
                System.Array.Empty<MovementScript>()
            );
            movementController.SetNewTarget((Transform)null);
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    public bool HasReachedReturnPosition()
    {
        if (!hasReturnPosition)
        {
            return false;
        }

        Vector3 currentPosition = rb != null
            ? rb.position
            : pathAgent != null
                ? pathAgent.transform.position
                : transform.position;

        Vector3 difference = returnPosition - currentPosition;
        difference.y = 0f;

        return difference.magnitude <= arrivalDistance ||
            (pathAgent != null && pathAgent.HasReachedDestination());
    }
}
