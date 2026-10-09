using UnityEngine;

public class SimpleMovementAnimator_NPC_Jabali_Jabato_Cocodrilo : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody rb;

    [Header("Animator States")]
    [SerializeField] private string idleState = "Idle";
    [SerializeField] private string walkState = "Walk";
    [SerializeField] private string runState = "Run";

    [Header("Movement")]
    [SerializeField] private float movementThreshold = 0.02f;
    [SerializeField] private float runThreshold = 2.75f;

    [Header("Animation Speed")]
    [SerializeField] private float walkReferenceSpeed = 2f;
    [SerializeField] private float runReferenceSpeed = 3f;
    [SerializeField] private float minAnimationSpeed = 0.65f;
    [SerializeField] private float maxAnimationSpeed = 1.5f;
    [SerializeField] private float transitionDuration = 0.1f;

    private enum MovementState
    {
        Idle,
        Walk,
        Run
    }

    private MovementState currentState = MovementState.Idle;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (rb == null)
            rb = GetComponent<Rigidbody>();

        if (animator != null)
            animator.applyRootMotion = false;
    }

    private void Update()
    {
        if (animator == null || rb == null)
            return;

        Vector3 horizontalVelocity = rb.linearVelocity;
        horizontalVelocity.y = 0f;

        float speed = horizontalVelocity.magnitude;

        MovementState newState;

        if (speed <= movementThreshold)
            newState = MovementState.Idle;
        else if (speed < runThreshold)
            newState = MovementState.Walk;
        else
            newState = MovementState.Run;

        if (newState != currentState)
        {
            currentState = newState;

            switch (currentState)
            {
                case MovementState.Idle:
                    animator.speed = 1f;
                    animator.CrossFade(idleState, transitionDuration);
                    break;

                case MovementState.Walk:
                    animator.CrossFade(walkState, transitionDuration);
                    break;

                case MovementState.Run:
                    animator.CrossFade(runState, transitionDuration);
                    break;
            }
        }

        if (currentState == MovementState.Walk)
        {
            animator.speed = Mathf.Clamp(
                speed / walkReferenceSpeed,
                minAnimationSpeed,
                maxAnimationSpeed
            );
        }
        else if (currentState == MovementState.Run)
        {
            animator.speed = Mathf.Clamp(
                speed / runReferenceSpeed,
                minAnimationSpeed,
                maxAnimationSpeed
            );
        }
        else
        {
            animator.speed = 1f;
        }
    }
}
