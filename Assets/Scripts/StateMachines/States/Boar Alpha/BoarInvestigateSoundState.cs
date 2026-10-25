using UnityEngine;

public class BoarInvestigateSoundState : State
{
    [SerializeField] private float arrivalDistance = 0.8f;
    [SerializeField] private float inspectionDuration = 3f;
    [SerializeField] private float stuckTimeout = 2f;

    private HearingSense_Boar hearingSense;
    private MovementSystemController movementController;
    private EnemyPathAgent pathAgent;
    private Steering_Face face;
    private Rigidbody body;
    private Vector3 soundPosition;
    private Vector3 closestPosition;
    private float timeWithoutProgress;
    private float inspectionTimer;
    private bool inspecting;

    public bool IsInspectionComplete { get; private set; }

    public override void OnEntry()
    {
        hearingSense = GetComponentInParent<HearingSense_Boar>();
        movementController = GetComponentInParent<MovementSystemController>();
        pathAgent = GetComponentInParent<EnemyPathAgent>();
        body = GetComponentInParent<Rigidbody>();
        face = GetComponentInParent<Steering_Face>();

        if (hearingSense == null)
        {
            IsInspectionComplete = true;
            return;
        }

        soundPosition = hearingSense.lastSoundPosition;
        soundPosition.y = transform.root.position.y;
        closestPosition = transform.root.position;
        timeWithoutProgress = 0f;
        inspectionTimer = 0f;
        inspecting = false;
        IsInspectionComplete = false;

        if (CanUsePathfinding())
        {
            pathAgent.SetDestination(soundPosition);
            if (face != null)
                face.targetTransform = pathAgent.PathLookTarget;
        }
        else
        {
            pathAgent?.ClearDestination();
            movementController?.SetNewTarget(soundPosition);

            if (face != null && pathAgent != null && pathAgent.PathLookTarget != null)
            {
                pathAgent.PathLookTarget.position = soundPosition;
                face.targetTransform = pathAgent.PathLookTarget;
            }
        }
    }

    public override void OnUpdate()
    {
        if (IsInspectionComplete)
            return;

        Vector3 currentPosition = transform.root.position;
        Vector3 offset = soundPosition - currentPosition;
        offset.y = 0f;

        if (!inspecting)
        {
            bool reached = CanUsePathfinding()
                ? pathAgent.HasReachedDestination()
                : offset.magnitude <= arrivalDistance;

            Vector3 progress = currentPosition - closestPosition;
            progress.y = 0f;

            if (progress.magnitude >= 0.05f)
            {
                closestPosition = currentPosition;
                timeWithoutProgress = 0f;
            }
            else
            {
                timeWithoutProgress += Time.deltaTime;
            }

            if (reached || timeWithoutProgress >= stuckTimeout)
            {
                inspecting = true;
                inspectionTimer = 0f;
                pathAgent?.ClearDestination();
                movementController?.SetNewTarget(currentPosition);

                if (body != null)
                    body.linearVelocity = Vector3.zero;

                if (face != null && pathAgent != null && pathAgent.PathLookTarget != null)
                {
                    pathAgent.PathLookTarget.position = soundPosition;
                    face.targetTransform = pathAgent.PathLookTarget;
                }
            }

            return;
        }

        inspectionTimer += Time.deltaTime;
        if (inspectionTimer >= inspectionDuration)
            IsInspectionComplete = true;
    }

    public override void OnExit()
    {
        pathAgent?.ClearDestination();

        if (body != null)
            body.linearVelocity = Vector3.zero;
    }

    private bool CanUsePathfinding()
    {
        return pathAgent != null &&
               pathAgent.isActiveAndEnabled &&
               pathAgent.gridManager != null &&
               pathAgent.gridManager.Grid != null &&
               pathAgent.pathfinding != null;
    }
}