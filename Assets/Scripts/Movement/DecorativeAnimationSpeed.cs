using UnityEngine;

public class DecorativeAnimationSpeed : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float animationSpeed = 1f;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (animator != null)
            animator.speed = animationSpeed;
    }
}