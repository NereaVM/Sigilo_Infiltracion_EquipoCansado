
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Grabbable : MonoBehaviour
{
    private Rigidbody rb;
    private Collider col;

    public bool IsBeingHeld { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    public void PickUp(Transform holdPoint)
    {
        IsBeingHeld = true;

        // Desactivar físicas mientras sostenemos el objeto.
        rb.isKinematic = true;
        rb.useGravity = false;

        // Evitar colisiones mientras está en la mano.
        if (col != null)
            col.enabled = false;

        transform.SetParent(holdPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void Throw(Vector3 force, Collider[] playerColliders = null)
    {
        IsBeingHeld = false;

        // Separar el objeto de la mano del jugador.
        transform.SetParent(null);

        // Activar las físicas.
        rb.isKinematic = false;
        rb.useGravity = true;

        // Eliminar velocidades anteriores.
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Recuperar las colisiones.
        if (col != null)
        {
            col.enabled = true;

            // Evitar que la piedra choque con el jugador.
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
                        Physics.IgnoreCollision(col, playerCol, true);
                    }
                }
            }
        }

        // Aplicar una velocidad inicial independiente de la masa.
        rb.AddForce(force, ForceMode.VelocityChange);

        // Avisar al objeto de que ha sido lanzado.
        RockImpactSound impactSound = GetComponent<RockImpactSound>();

        if (impactSound != null)
            impactSound.MarkAsThrown();
    }
}
