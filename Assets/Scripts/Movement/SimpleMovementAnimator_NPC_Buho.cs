using UnityEngine;

public class SimpleMovementAnimator_NPC_Buho : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody rb;

    [Header("Animator States")]
    [SerializeField] private string idleState = "Idle";
    [SerializeField] private string movingState = "Jump";

    [Header("Movement")]
    [SerializeField] private float movementThreshold = 0.02f;
    [SerializeField] private float referenceSpeed = 2f;
    [SerializeField] private float minAnimationSpeed = 0.65f;
    [SerializeField] private float maxAnimationSpeed = 1.5f;
    [SerializeField] private float transitionDuration = 0.1f;

    private bool isMoving;

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
        bool shouldMove = speed > movementThreshold;

        if (shouldMove != isMoving)
        {
            isMoving = shouldMove;

            if (isMoving)
                animator.CrossFade(movingState, transitionDuration);
            else
            {
                animator.speed = 1f;
                animator.CrossFade(idleState, transitionDuration);
            }
        }

        if (isMoving)
        {
            animator.speed = Mathf.Clamp(
                speed / referenceSpeed,
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
