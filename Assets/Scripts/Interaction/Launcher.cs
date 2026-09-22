using UnityEngine;
using UnityEngine.InputSystem;

public class Launcher : MonoBehaviour
{
    public Transform spawnPoint;
    public float pickupRange = 4f;
    public float maxThrowDistance = 8f;
    public float throwForceMultiplier = 2.2f;
    public float upwardForce = 3.5f;

    private Camera mainCamera;
    private Grabbable currentItem;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Mouse.current == null)
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (currentItem == null)
            {
                TryPickUpObject();
            }
            else
            {
                ThrowObject();
            }
        }
    }

    private void TryPickUpObject()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Grabbable grabbable = hit.collider.GetComponentInParent<Grabbable>();

            if (grabbable == null || grabbable.IsBeingHeld)
                return;

            float distanceToPlayer = Vector3.Distance(transform.position, grabbable.transform.position);

            if (distanceToPlayer <= pickupRange)
            {
                currentItem = grabbable;
                currentItem.PickUp(spawnPoint != null ? spawnPoint : transform);
            }
        }
    }

    private void ThrowObject()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 origin = spawnPoint != null ? spawnPoint.position : transform.position;
            Vector3 target = hit.point;

            Vector3 direction = target - origin;
            direction.y = 0f;

            float distance = direction.magnitude;
            float clampedDistance = Mathf.Clamp(distance, 1f, maxThrowDistance);
            Vector3 normalizedDirection = direction.normalized;

            Vector3 horizontalForce = normalizedDirection * (clampedDistance * throwForceMultiplier);
            Vector3 verticalForce = Vector3.up * upwardForce;
            Vector3 finalForce = horizontalForce + verticalForce;

            currentItem.Throw(finalForce);
            currentItem = null;
        }
    }
}
