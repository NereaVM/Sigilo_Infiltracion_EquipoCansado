using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BushSafeZone : MonoBehaviour
{
    private void Reset()
    {
        Collider zoneCollider =
            GetComponent<Collider>();

        if (zoneCollider != null)
        {
            zoneCollider.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerBushStealth stealth =
            other.GetComponentInParent<PlayerBushStealth>();

        if (stealth != null)
        {
            stealth.EnterBush();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerBushStealth stealth =
            other.GetComponentInParent<PlayerBushStealth>();

        if (stealth != null)
        {
            stealth.ExitBush();
        }
    }
}
