using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
//using UnityEngine.InputSystem;

public class EnemyPlaceholderScript : MonoBehaviour
{
    enum EnemyState
    {
        Idle,
        SearchingSound,
        PursuingPlayer
    }

    public bool isDebugging = false;

    private Vector3 targetPosition;
    private Rigidbody rb;
    private HearingSense_PlaceholderEnemy hearingSense;

    // NEREA: Reference to the independent vision component.
    private VisionSense visionSense;

    private int currentSoundPriority = -1;
    [SerializeField] private EnemyState currentState = EnemyState.Idle;
    [SerializeField] private GeneratedSound alertSoundPrefab; // Reference to the alert sound prefab
    private Transform playerTransform;

    public float moveSpeed = 2f;
    private float moveTimer = 0f;

    bool isMoving = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        hearingSense = GetComponent<HearingSense_PlaceholderEnemy>();

        // NEREA: Get the vision component in the same way as the hearing component.
        visionSense = GetComponent<VisionSense>();

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void ChangeState(EnemyState newState, Vector3? soundPosition = null)
    {
        currentState = newState;

        switch (newState)
        {
            case EnemyState.Idle:
                Debug.Log($"Enemy is now idle. {gameObject}[EnemyPlaceholderScript][ChangeState]");
                isMoving = false;
                currentSoundPriority = -1;
                break;

            case EnemyState.SearchingSound:
                Debug.Log($"Enemy is now searching for sound. {gameObject}[EnemyPlaceholderScript][ChangeState]");
                targetPosition = (Vector3)soundPosition;
                isMoving = true;
                break;

            case EnemyState.PursuingPlayer:
                Debug.Log($"Enemy is now pursuing the player. {gameObject}[EnemyPlaceholderScript][ChangeState]");
                isMoving = true;
                AlertEnemies();
                break;
        }
    }

    void AlertEnemies()
    {
        Object alertSound = Instantiate(
            alertSoundPrefab,
            this.transform,
            instantiateInWorldSpace: false
        );

        alertSound.GetComponent<GeneratedSound>().soundGenerator = hearingSense;

        Debug.Log(
            $"Enemy has alerted other enemies. {gameObject}[EnemyPlaceholderScript][AlertEnemies]"
        );
    }

    void InvestigateSound(int priority, Vector3 soundPosition)
    {
        if (priority >= currentSoundPriority)
        {
            ChangeState(
                EnemyState.SearchingSound,
                soundPosition: soundPosition
            );
        }
    }

    void PursuePlayer()
    {
        ChangeState(EnemyState.PursuingPlayer);
    }

    // Update is called once per frame
    void Update()
    {
        CheckSenses();

        if (currentState == EnemyState.Idle)
        {
            MoveRandomly();
        }

        if (currentState == EnemyState.PursuingPlayer)
        {
            //targetPosition = new Vector3(
            //    playerTransform.position.x,
            //    transform.position.y,
            //    playerTransform.position.z
            //);

            // Instead of going back to idle, enemy stays still for showcase purposes
            if (Vector3.Distance(transform.position, playerTransform.position) < 2f && isMoving)
            {
                StartCoroutine(WaitAndReturnToIdle());
            }
        }

        if (isMoving)
        {
            MoveTowardsTarget();
        }
    }

    private IEnumerator WaitAndReturnToIdle()
    {
        isMoving = false;

        yield return new WaitForSeconds(2f);

        // NEREA:
        // If the player is still visible after the wait,
        // the enemy remains still in PursuingPlayer.
        // This prevents repeatedly entering the state and generating alerts.
        if (visionSense != null && visionSense.hasSeenEntity)
        {
            yield break;
        }

        ChangeState(EnemyState.Idle);
    }

    void CheckSenses()
    {
        CheckHearingSense();

        // NEREA: Vision is checked independently from hearing.
        CheckVisionSense();
    }

    void CheckHearingSense()
    {
        if (hearingSense == null) return;

        if (currentState == EnemyState.PursuingPlayer)
        {
            hearingSense.ResetHearing();
            return;
        }

        if (hearingSense.hasHeardSound)
        {
            Debug.Log(
                $"Enemy has heard a sound. {gameObject} " +
                "[EnemyPlaceholderScript][CheckHearingSense]"
            );

            if (hearingSense.lastSoundType == HearingSystem.SoundType.OwlAlert)
            {
                PursuePlayer();
            }
            else
            {
                InvestigateSound(
                    hearingSense.GetPriority(hearingSense.lastSoundType),
                    new Vector3(
                        hearingSense.lastSoundPosition.x,
                        transform.position.y,
                        hearingSense.lastSoundPosition.z
                    )
                );
            }

            hearingSense.ResetHearing();
        }
    }

    // NEREA:
    // Replaces the old keyboard vision placeholder.
    // The enemy asks VisionSense if an entity is currently visible.
    void CheckVisionSense()
    {
        if (visionSense == null) return;

        if (visionSense.hasSeenEntity)
        {
            // NEREA:
            // The target comes from the vision component instead of directly
            // reading the player's position.
            targetPosition = new Vector3(
                visionSense.lastSeenPosition.x,
                transform.position.y,
                visionSense.lastSeenPosition.z
            );

            // NEREA:
            // Only enter PursuingPlayer once.
            // This prevents AlertEnemies() from being executed every frame.
            if (currentState != EnemyState.PursuingPlayer)
            {
                Debug.Log(
                    $"Enemy has seen an entity. {gameObject} " +
                    "[EnemyPlaceholderScript][CheckVisionSense]"
                );

                PursuePlayer();
            }
        }
        else
        {
            // NEREA:
            // If the enemy is already stopped and the entity is no longer visible,
            // return to the original Idle behaviour.
            if (currentState == EnemyState.PursuingPlayer && !isMoving)
            {
                ChangeState(EnemyState.Idle);
            }
        }

        /*
        // Original placeholder used before the real vision system was implemented.
        var keyboard = Keyboard.current;

        if (keyboard.vKey.wasPressedThisFrame && isDebugging)
        {
            if (currentState == EnemyState.PursuingPlayer)
            {
                ChangeState(EnemyState.Idle);
            }
            else
            {
                PursuePlayer();
            }
        }
        */
    }

    void MoveRandomly()
    {
        moveTimer += Time.deltaTime;

        if ((moveTimer >= 3f && Random.value < 0.5f) || !isMoving)
        {
            moveTimer = 0f;

            targetPosition = new Vector3(
                Random.Range(-20f, 20f),
                transform.position.y,
                Random.Range(-20f, 20f)
            );

            isMoving = true;
        }
    }

    void MoveTowardsTarget()
    {
        // NEREA:
        // Rotate the enemy on the Y axis so its forward direction
        // (and therefore its vision cone) points towards the movement target.
        Vector3 directionToTarget = targetPosition - transform.position;
        directionToTarget.y = 0f;

        if (directionToTarget.sqrMagnitude > 0.001f)
        {
            rb.MoveRotation(
                Quaternion.LookRotation(directionToTarget.normalized)
            );
        }        
        
        rb.MovePosition(
            Vector3.MoveTowards(
                transform.position,
                targetPosition,
                Time.deltaTime * moveSpeed
            )
        );

        if (Vector3.Distance(transform.position, targetPosition) < 0.5f)
        {
            isMoving = false;

            if (currentState == EnemyState.SearchingSound)
            {
                ChangeState(EnemyState.Idle);

                Debug.Log(
                    $"Enemy has reached the sound position. {gameObject} " +
                    "[EnemyPlaceholderScript][MoveTowardsTarget]"
                );
            }
        }
    }

    void OnDrawGizmos()
    {
        if (currentState == EnemyState.SearchingSound)
        {
            Gizmos.color = Color.green;
        }
        else if (currentState == EnemyState.PursuingPlayer)
        {
            Gizmos.color = Color.red;
        }
        else if (currentState == EnemyState.Idle)
        {
            Gizmos.color = Color.blue;
        }

        Gizmos.DrawWireSphere(targetPosition, 0.5f);
        Gizmos.DrawLine(transform.position, targetPosition);
    }
}