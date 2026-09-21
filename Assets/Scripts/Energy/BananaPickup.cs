using UnityEngine;

public class BananaPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerEnergy playerEnergy =
            other.GetComponentInParent<PlayerEnergy>();

        if (playerEnergy == null)
            return;

        playerEnergy.RestoreEnergy();

        Debug.Log("Banana recogida: energía restaurada al máximo.");

        Destroy(gameObject);
    }
}