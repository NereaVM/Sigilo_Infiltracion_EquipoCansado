using UnityEngine;

public class SimpleMovementAnimator_Player_Mono : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerMovement playerMovement;

    [Header("Animator States")]
    [SerializeField] private string idleState = "Idle";
    [SerializeField] private string walkState = "Walk";
    [SerializeField] private string runState = "Run";

    [Header("Movement")]
    [SerializeField] private float movementThreshold = 0.02f;
    [SerializeField] private float runThreshold = 2.75f;

    [Header("Animation Speed")]
    [SerializeField] private float walkReferenceSpeed = 2f;
    [SerializeField] private float runReferenceSpeed = 4f;
    [SerializeField] private float minAnimationSpeed = 0.65f;
    [SerializeField] private float maxAnimationSpeed = 1.5f;

    [Header("Transitions")]
    [SerializeField] private float transitionDuration = 0.1f;

    // NEREA:
    // Slightly longer transition when the monkey becomes tired
    // and changes from running to walking.
    [SerializeField] private float runToWalkTransitionDuration = 0.3f;

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

        if (playerMovement == null)
            playerMovement = GetComponent<PlayerMovement>();

        if (animator != null)
            animator.applyRootMotion = false;
    }

    private void Update()
    {
        if (animator == null || playerMovement == null)
            return;

        float speed = playerMovement.CurrentMoveSpeed;

        MovementState newState;

        if (speed <= movementThreshold)
        {
            newState = MovementState.Idle;
        }
        else if (speed < runThreshold)
        {
            newState = MovementState.Walk;
        }
        else
        {
            newState = MovementState.Run;
        }

        if (newState != currentState)
        {
            MovementState previousState = currentState;
            currentState = newState;

            switch (currentState)
            {
                case MovementState.Idle:
                    animator.speed = 1f;

                    animator.CrossFadeInFixedTime(
                        idleState,
                        transitionDuration
                    );
                    break;

                case MovementState.Walk:

                    // NEREA:
                    // Run -> Walk uses a smoother transition
                    // to avoid a visible cut when stamina decreases.
                    float walkTransition =
                        previousState == MovementState.Run
                        ? runToWalkTransitionDuration
                        : transitionDuration;

                    animator.CrossFadeInFixedTime(
                        walkState,
                        walkTransition
                    );
                    break;

                case MovementState.Run:
                    animator.CrossFadeInFixedTime(
                        runState,
                        transitionDuration
                    );
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