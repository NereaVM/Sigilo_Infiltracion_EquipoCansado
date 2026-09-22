using System.Collections.Generic;
using UnityEngine;

public class VisionSense : MonoBehaviour
{
    [Header("Vision Parameters")]
    [SerializeField] private float viewDistance = 10f;

    [Range(0f, 360f)]
    [SerializeField] private float viewAngle = 90f;

    [SerializeField] private Transform visionOrigin;

    [SerializeField] private LayerMask detectionMask = ~0;

    public bool hasSeenEntity { get; private set; }
    public Vector3 lastSeenPosition { get; private set; }
    public VisibleEntity lastSeenEntity { get; private set; }

    void Update()
    {
        CheckVision();
    }

    void CheckVision()
    {
        // Vision is recalculated every frame.
        // If nothing is visible this frame, hasSeenEntity stays false.
        hasSeenEntity = false;
        
        Vector3 origin = GetVisionOrigin();

        Collider[] detectedColliders = Physics.OverlapSphere(
            origin,
            viewDistance,
            detectionMask,
            QueryTriggerInteraction.Ignore
        );

        HashSet<VisibleEntity> checkedEntities = new HashSet<VisibleEntity>();

        VisibleEntity closestVisibleEntity = null;
        float closestDistance = float.MaxValue;

        foreach (Collider detectedCollider in detectedColliders)
        {
            VisibleEntity visibleEntity =
                detectedCollider.GetComponentInParent<VisibleEntity>();

            if (visibleEntity == null)
            {
                continue;
            }

            if (!visibleEntity.CanBeSeen)
            {
                continue;
            }

            if (!checkedEntities.Add(visibleEntity))
            {
                continue;
            }

            // Prevents the entity from detecting itself
            if (visibleEntity.transform.root == transform.root)
            {
                continue;
            }

            Vector3 targetPosition = visibleEntity.GetVisionPosition();
            Vector3 directionToTarget = targetPosition - origin;

            float distanceToTarget = directionToTarget.magnitude;

            if (distanceToTarget <= Mathf.Epsilon)
            {
                continue;
            }

            Vector3 horizontalDirection =
                Vector3.ProjectOnPlane(directionToTarget, Vector3.up);

            Vector3 horizontalForward =
                Vector3.ProjectOnPlane(GetVisionForward(), Vector3.up);

            if (horizontalDirection.sqrMagnitude <= 0.0001f ||
                horizontalForward.sqrMagnitude <= 0.0001f)
            {
                continue;
            }

            float angleToTarget = Vector3.Angle(
                horizontalForward.normalized,
                horizontalDirection.normalized
            );

            if (angleToTarget > viewAngle * 0.5f)
            {
                continue;
            }

            // Raycast: checks that no obstacle is between the observer
            // and the visible entity.
            if (Physics.Raycast(
                origin,
                directionToTarget.normalized,
                out RaycastHit hit,
                distanceToTarget,
                detectionMask,
                QueryTriggerInteraction.Ignore))
            {
                VisibleEntity hitEntity =
                    hit.collider.GetComponentInParent<VisibleEntity>();

                if (hitEntity != visibleEntity)
                {
                    continue;
                }
            }
            else
            {
                continue;
            }

            if (distanceToTarget < closestDistance)
            {
                closestDistance = distanceToTarget;
                closestVisibleEntity = visibleEntity;
            }
        }

        if (closestVisibleEntity != null)
        {
            hasSeenEntity = true;
            lastSeenEntity = closestVisibleEntity;
            lastSeenPosition = closestVisibleEntity.GetVisionPosition();

            Debug.Log(
                $"Entity seen: {closestVisibleEntity.gameObject.name}. " +
                $"{gameObject}[VisionSense][CheckVision]"
            );
        }
    }

    Vector3 GetVisionOrigin()
    {
        if (visionOrigin != null)
        {
            return visionOrigin.position;
        }

        return transform.position;
    }

    Vector3 GetVisionForward()
    {
        if (visionOrigin != null)
        {
            return visionOrigin.forward;
        }

        return transform.forward;
    }

    public void ResetVision()
    {
        hasSeenEntity = false;
        lastSeenEntity = null;
    }

    public void SetVisionEnabled(bool value)
    {
        enabled = value;

        if (!value)
        {
            ResetVision();
        }
    }

    void OnDisable()
    {
        ResetVision();
    }

    void OnDrawGizmosSelected()
    {
        Vector3 origin = GetVisionOrigin();

        Gizmos.color = Color.cyan;

        Gizmos.DrawWireSphere(origin, viewDistance);

        Vector3 forward = GetVisionForward();

        Vector3 leftBoundary =
            Quaternion.AngleAxis(-viewAngle * 0.5f, Vector3.up) * forward;

        Vector3 rightBoundary =
            Quaternion.AngleAxis(viewAngle * 0.5f, Vector3.up) * forward;

        Gizmos.DrawRay(origin, leftBoundary * viewDistance);
        Gizmos.DrawRay(origin, rightBoundary * viewDistance);
    }
}