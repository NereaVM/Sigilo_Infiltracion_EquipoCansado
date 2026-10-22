
using UnityEngine;

public class RockImpactSound : MonoBehaviour
{
    [Header("Sound")]
    [SerializeField] private GeneratedSound soundPrefab;
    [SerializeField] private float minImpactSpeed = 1.5f;
    [SerializeField] private LayerMask impactLayers;

    private bool hasBeenThrown = false;

    public void MarkAsThrown()
    {
        hasBeenThrown = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!hasBeenThrown || soundPrefab == null)
            return;

        // Comprobar que la superficie puede generar ruido.
        if ((impactLayers.value & (1 << collision.gameObject.layer)) == 0)
            return;

        // Evitar generar sonidos por impactos demasiado suaves.
        if (collision.relativeVelocity.magnitude < minImpactSpeed)
            return;

        hasBeenThrown = false;

        // Obtener la posición del impacto.
        Vector3 soundPosition = collision.contactCount > 0
            ? collision.GetContact(0).point
            : transform.position;

        // Generar el aviso para los NPC.
        Instantiate(soundPrefab, soundPosition, Quaternion.identity);
    }
}
