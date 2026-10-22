
using UnityEngine;
using UnityEngine.InputSystem;

public class Launcher : MonoBehaviour
{
    [Header("Recogida")]
    public float pickupRange = 4f;

    [Header("Lanzamiento")]
    public Transform spawnPoint;
    public float minThrowForce = 5f;
    public float maxThrowForce = 12f;
    public float maxChargeTime = 2f;
    public float upwardForce = 6f;

    private Grabbable currentItem;
    private float chargeStartTime = -1f;

    // Colliders del jugador.
    private Collider[] playerColliders;

    public bool IsCharging =>
        currentItem != null && chargeStartTime >= 0f;

    public float ChargeNormalized =>
        IsCharging
            ? Mathf.Clamp01(
                (Time.time - chargeStartTime) /
                Mathf.Max(0.01f, maxChargeTime))
            : 0f;

    private void Awake()
    {
        // Obtener los colliders del jugador y sus hijos.
        playerColliders = GetComponentsInChildren<Collider>();
    }

    private void Update()
    {
        if (Keyboard.current == null) return;

        bool rPressed = Keyboard.current.rKey.wasPressedThisFrame;
        bool rReleased = Keyboard.current.rKey.wasReleasedThisFrame;

        // Sin objeto: recoger al pulsar R.
        if (currentItem == null)
        {
            if (rPressed)
                TryPickUp();

            return;
        }

        // Empezar a cargar el lanzamiento.
        if (rPressed)
        {
            chargeStartTime = Time.time;
        }

        // Lanzar al soltar R.
        if (rReleased && chargeStartTime >= 0f)
        {
            ThrowObject();
        }
    }

    private void TryPickUp()
    {
        Collider[] hits =
            Physics.OverlapSphere(transform.position, pickupRange);

        Grabbable closest = null;
        float closestDist = float.MaxValue;

        foreach (var col in hits)
        {
            Grabbable g = col.GetComponentInParent<Grabbable>();

            if (g == null || g.IsBeingHeld)
                continue;

            float dist = Vector3.Distance(
                transform.position,
                g.transform.position
            );

            if (dist < closestDist)
            {
                closestDist = dist;
                closest = g;
            }
        }

        if (closest == null) return;

        currentItem = closest;

        currentItem.PickUp(
            spawnPoint != null ? spawnPoint : transform
        );
    }

    private void ThrowObject()
    {
        // Dirección horizontal hacia donde mira el jugador.
        Vector3 throwDir = transform.forward;
        throwDir.y = 0f;
        throwDir.Normalize();

        // Intensidad según cuánto hemos cargado.
        float force = Mathf.Lerp(
            minThrowForce,
            maxThrowForce,
            ChargeNormalized
        );

        // Movimiento hacia delante y hacia arriba.
        Vector3 finalForce =
            throwDir * force +
            Vector3.up * upwardForce;

        // Lanzar evitando colisiones con el jugador.
        currentItem.Throw(finalForce, playerColliders);

        currentItem = null;
        chargeStartTime = -1f;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}
