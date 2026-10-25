using UnityEngine;

public class CrocodilePursuitState : State
{
    [Header("Pursuit")]
    [SerializeField] private Transform target;

    [Header("Wall Impact")]
    [SerializeField] private float minimumImpactSpeed = 4f;
    [SerializeField] private float maximumImpactAngle = 45f;
    [SerializeField] private float recoveryDuration = 2f;

    private MovementSystemController movementController;
    private EnemyPathAgent pathAgent;
    private VisionSense vision;
    private Rigidbody rb;
    private bool usingPathfinding;
    private bool recoveringFromImpact;
    private float recoveryEndTime;
    private Collider crocodileCollider;
    private bool originalIsKinematic;
    private Vector3 impactNormal;

    public override void OnEntry()
    {
        movementController =
            GetComponentInParent<MovementSystemController>();
        pathAgent =
            GetComponentInParent<EnemyPathAgent>();
        vision =
            GetComponentInParent<VisionSense>();
        rb =
            GetComponentInParent<Rigidbody>();
        if (rb != null)
        {
            originalIsKinematic = rb.isKinematic;
        }
        crocodileCollider =
            GetComponentInParent<Collider>();

        recoveringFromImpact = false;
        ConfigurePursuit();
    }

    public override void OnExit()
    {
        EndImpactRecovery();

        if (pathAgent != null && usingPathfinding)
        {
            pathAgent.ClearDestination();
        }

        if (movementController != null)
        {
            movementController.SetNewTarget((Transform)null);
        }

        usingPathfinding = false;
        recoveringFromImpact = false;
    }

    public override void OnUpdate()
    {
        if (recoveringFromImpact)
        {
            if (Time.time >= recoveryEndTime)
            {
                EndImpactRecovery();
                ConfigurePursuit();
            }

            return;
        }

        if (HasHitWall(out impactNormal))
        {
            BeginImpactRecovery();
            return;
        }

        if (vision != null && vision.hasSeenEntity)
        {
            PursueVisibleTarget();
        }
        else if (!usingPathfinding)
        {
            ConfigurePursuit();
        }
    }

    private void ConfigurePursuit()
    {
        Transform pursuitTarget = GetPursuitTarget();

        if (movementController == null || pursuitTarget == null)
        {
            Debug.LogWarning(
                "Crocodile Pursuit: A pursuit target and movement controller " +
                "are required."
            );
            return;
        }

        movementController.ChangeToMovementScripts(movementScripts);

        if (vision != null && vision.hasSeenEntity)
        {
            PursueVisibleTarget();
            return;
        }

        usingPathfinding =
            pathAgent != null &&
            pathAgent.isActiveAndEnabled &&
            pathAgent.gridManager != null &&
            pathAgent.pathfinding != null;

        if (usingPathfinding)
        {
            movementController.currentTargetTransform = null;
            ClearMovementTargetTransforms();
            pathAgent.SetDestination(pursuitTarget);

            foreach (MovementScript movementScript in movementScripts)
            {
                if (movementScript is Steering_Face face)
                {
                    face.targetTransform = pathAgent.PathLookTarget;
                }
            }
        }
        else
        {
            usingPathfinding = false;
            movementController.SetNewTarget(pursuitTarget);
            SetMovementTargetTransforms(pursuitTarget);
        }
    }

    private void PursueVisibleTarget()
    {
        if (movementController == null)
        {
            return;
        }

        if (usingPathfinding && pathAgent != null)
        {
            pathAgent.ClearDestination();
            usingPathfinding = false;
        }

        movementController.SetNewTarget(vision.lastSeenPosition);
        movementController.currentTargetTransform = null;
        ClearMovementTargetTransforms();
    }

    private Transform GetPursuitTarget()
    {
        if (target != null)
        {
            return target;
        }

        if (vision != null && vision.lastSeenEntity != null)
        {
            target = vision.lastSeenEntity.transform;
        }

        return target;
    }

    private void ClearMovementTargetTransforms()
    {
        foreach (MovementScript movementScript in movementScripts)
        {
            if (movementScript != null)
            {
                movementScript.targetTransform = null;
            }
        }
    }

    private void SetMovementTargetTransforms(Transform targetTransform)
    {
        foreach (MovementScript movementScript in movementScripts)
        {
            if (movementScript != null)
            {
                movementScript.targetTransform = targetTransform;
            }
        }
    }

    private bool HasHitWall(out Vector3 hitNormal)
    {
        hitNormal = Vector3.zero;

        if (rb == null || rb.linearVelocity.magnitude < minimumImpactSpeed)
        {
            return false;
        }

        LayerMask obstacleMask =
            pathAgent != null && pathAgent.gridManager != null
            ? pathAgent.gridManager.obstacleMask
            : Physics.DefaultRaycastLayers;

        Vector3 velocity = rb.linearVelocity;
        Vector3 velocityDirection = velocity.normalized;
        float castDistance =
            velocity.magnitude * Time.fixedDeltaTime + 0.1f;
        float castRadius =
            crocodileCollider != null
            ? Mathf.Max(
                crocodileCollider.bounds.extents.x,
                crocodileCollider.bounds.extents.z
            )
            : 0.3f;

        if (!Physics.SphereCast(
            rb.position,
            castRadius,
            velocityDirection,
            out RaycastHit hit,
            castDistance,
            obstacleMask,
            QueryTriggerInteraction.Ignore))
        {
            return false;
        }

        float impactAngle =
            Vector3.Angle(velocityDirection, -hit.normal);

        if (impactAngle > maximumImpactAngle)
        {
            return false;
        }

        hitNormal = hit.normal;
        return true;
    }

    private void BeginImpactRecovery()
    {
        recoveringFromImpact = true;
        recoveryEndTime = Time.time + Mathf.Max(0f, recoveryDuration);

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.position += impactNormal * 0.05f;
        }

        if (movementController != null)
        {
            movementController.ChangeToMovementScripts(
                System.Array.Empty<MovementScript>()
            );
        }
    }

    private void EndImpactRecovery()
    {
        if (!recoveringFromImpact)
        {
            return;
        }

        recoveringFromImpact = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = originalIsKinematic;
        }
    }
}