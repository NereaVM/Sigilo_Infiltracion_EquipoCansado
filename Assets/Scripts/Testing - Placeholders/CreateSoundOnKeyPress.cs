using UnityEngine;
using UnityEngine.InputSystem;

public class CreateSoundOnKeyPress : MonoBehaviour
{

    public GameObject soundPrefab; // Reference to the sound prefab
    public float spawnRadius = 5f; // Radius within which the sound will be spawned
    private Vector3 lastSpawnPosition;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var keyboard = Keyboard.current;

        if (keyboard.spaceKey.wasPressedThisFrame)
        {
            lastSpawnPosition = RandomizeSpawnPosition();
            // Instantiate the sound prefab at the position of this GameObject
            Instantiate(soundPrefab, lastSpawnPosition, Quaternion.identity);
        }
    }

    private void OnDrawGizmos()
    {
        if (soundPrefab == null)
        {
            return;
        }

        SphereCollider sphereCollider = soundPrefab.GetComponentInChildren<SphereCollider>();
        if (sphereCollider == null)
        {
            return;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(lastSpawnPosition, sphereCollider.radius * sphereCollider.transform.lossyScale.x);
    }

    Vector3 RandomizeSpawnPosition()
    {
        return new Vector3(Random.Range(-spawnRadius, spawnRadius), 0, Random.Range(-spawnRadius, spawnRadius));
    }
}
