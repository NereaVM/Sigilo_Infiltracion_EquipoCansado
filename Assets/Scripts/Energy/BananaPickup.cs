using UnityEngine;

public class BananaPickup : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioClip pickupSound;

    [Range(0f, 1f)]
    [SerializeField] private float pickupVolume = 1f;

    private void OnTriggerEnter(Collider other)
    {
        PlayerEnergy playerEnergy =
            other.GetComponentInParent<PlayerEnergy>();

        if (playerEnergy == null)
            return;

        playerEnergy.RestoreEnergy();

        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(
                pickupSound,
                transform.position,
                pickupVolume
            );
        }

        Debug.Log("Banana recogida: energía restaurada al máximo.");

        Destroy(gameObject);
    }
}