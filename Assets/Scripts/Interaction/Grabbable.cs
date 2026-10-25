
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Grabbable : MonoBehaviour
{
    [Header("Configuración de la piedra")]
    [SerializeField] private bool freezeWhenResting = true;

    [Tooltip("Tiempo mínimo antes de comprobar si la piedra se ha detenido.")]
    [SerializeField] private float minimumFlightTime = 0.5f;

    private Rigidbody rb;
    private Collider col;

    private bool waitingToSettle = false;
    private float timeSinceThrow = 0f;

    private Collider[] ignoredPlayerColliders;

    public bool IsBeingHeld { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        // La piedra comienza inmóvil.
        if (freezeWhenResting)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
            col.isTrigger = true;
        }
    }

    private void FixedUpdate()
    {
        // Solo comprobar si fue lanzada.
        if (!waitingToSettle || IsBeingHeld)
            return;

        timeSinceThrow += Time.fixedDeltaTime;

        // Permitir que complete su lanzamiento.
        if (timeSinceThrow < minimumFlightTime)
            return;

        // Esperar a que Unity considere
        // que el Rigidbody está en reposo.
        if (rb.IsSleeping())
        {
            waitingToSettle = false;

            // Volver a dejar la piedra fija.
            rb.isKinematic = true;
            rb.useGravity = false;
            col.isTrigger = true;

            // Restaurar colisiones con el jugador.
            RestorePlayerCollisions();

            Debug.Log(
                "[Grabbable] Piedra detenida. " +
                "Is Kinematic activado."
            );
        }
    }

    public void PickUp(Transform holdPoint)
    {
        IsBeingHeld = true;
        waitingToSettle = false;

        // Detener cualquier movimiento anterior.
        if (!rb.isKinematic)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Desactivar físicas mientras está cogida.
        rb.isKinematic = true;
        rb.useGravity = false;

        // Evitar colisiones con el jugador.
        if (col != null)
        {
            col.enabled = false;
        }

        ignoredPlayerColliders = null;

        // Colocar la piedra en la mano.
        transform.SetParent(holdPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void Throw(
        Vector3 force,
        Collider[] playerColliders = null)
    {
        IsBeingHeld = false;

        // Separar la piedra del jugador.
        transform.SetParent(null);

        // Activar físicas para el lanzamiento.
        rb.isKinematic = false;
        rb.useGravity = true;

        // Eliminar velocidades anteriores.
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.WakeUp();

        // Recuperar colisiones.
        if (col != null)
        {
            col.isTrigger = false;
            col.enabled = true;

            ignoredPlayerColliders = playerColliders;

            // Evitar colisión inmediata con el jugador.
            if (playerColliders != null)
            {
                foreach (Collider playerCol in playerColliders)
                {
                    if (playerCol != null &&
                        playerCol.enabled &&
                        playerCol.gameObject.activeInHierarchy &&
                        !playerCol.isTrigger &&
                        playerCol != col)
                    {
                        Physics.IgnoreCollision(
                            col,
                            playerCol,
                            true
                        );
                    }
                }
            }
        }

        // Lanzamiento independiente de la masa.
        rb.AddForce(force, ForceMode.VelocityChange);

        // Comenzar comprobación de reposo.
        if (freezeWhenResting)
        {
            waitingToSettle = true;
            timeSinceThrow = 0f;
        }

        // Mantener el sistema de sonido.
        RockImpactSound impactSound =
            GetComponent<RockImpactSound>();

        if (impactSound != null)
        {
            impactSound.MarkAsThrown();
        }
    }

    private void RestorePlayerCollisions()
    {
        if (col == null || ignoredPlayerColliders == null)
            return;

        foreach (Collider playerCol in ignoredPlayerColliders)
        {
            if (playerCol != null &&
                playerCol.enabled &&
                playerCol.gameObject.activeInHierarchy)
            {
                Physics.IgnoreCollision(
                    col,
                    playerCol,
                    false
                );
            }
        }

        ignoredPlayerColliders = null;
    }
}
