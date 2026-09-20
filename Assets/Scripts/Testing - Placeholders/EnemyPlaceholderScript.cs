using UnityEngine;

public class EnemyPlaceholderScript : MonoBehaviour
{

    private Vector3 targetPosition;
    private Rigidbody rb;
    private HearingSense_PlaceholderEnemy hearingSense;

    public float moveSpeed = 2f;
    private float moveTimer = 0f;

    public bool isSearchingSound = false;


    bool isMoving = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        hearingSense = GetComponent<HearingSense_PlaceholderEnemy>();
    }

    // Update is called once per frame
    void Update()
    {

        if (hearingSense != null && hearingSense.hasHeardSound)
        {
            SearchForSound(hearingSense.lastSoundPosition);
            hearingSense.ResetHearing();
        }

        if (!isSearchingSound)
        {
            moveTimer += Time.deltaTime;
            if (moveTimer >= 3f && Random.value < 0.5f)
            {
                moveTimer = 0f;
                isMoving = false;
            }


            if (!isMoving) 
            {
                targetPosition = new Vector3(Random.Range(-20f, 20f), transform.position.y, Random.Range(-20f, 20f));
                isMoving = true;
            }
        }


        if (isMoving)
        {
            rb.MovePosition(Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * moveSpeed));
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f) {
                isMoving = false;
                if (isSearchingSound) isSearchingSound = false;
            }
        }
    }

    void OnDrawGizmos()
    {
        if (isSearchingSound)
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.red;
        }

        Gizmos.DrawWireSphere(targetPosition, 0.5f);
        Gizmos.DrawLine(transform.position, targetPosition);
    }

    public void SearchForSound(Vector3 soundPosition)
    {
        targetPosition = soundPosition;
        isSearchingSound = true;
        isMoving = true;
    }
    
}
