using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float rotationSpeed = 10f;

    private Rigidbody rb;
    private Vector2 movementInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // El jugador solo se mueve por el suelo.
        rb.useGravity = false;

        // Evitamos movimiento vertical y giros indeseados.
        rb.constraints =
            RigidbodyConstraints.FreezePositionY |
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationZ;
    }

    private void FixedUpdate()
    {
        Move();
    }

    public void SetMovementInput(Vector2 input)
    {
        // Evita que las diagonales sean más rápidas.
        movementInput = Vector2.ClampMagnitude(input, 1f);
    }

    private void Move()
    {
        Vector3 direction = new Vector3(
            movementInput.x,
            0f,
            movementInput.y
        );

        if (direction.sqrMagnitude < 0.001f)
            return;

        Vector3 movement =
            direction * moveSpeed * Time.fixedDeltaTime;

        rb.MovePosition(rb.position + movement);

        // El jugador mira hacia donde camina.
        Quaternion targetRotation =
            Quaternion.LookRotation(direction, Vector3.up);

        Quaternion newRotation =
            Quaternion.Slerp(
                rb.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime
            );

        rb.MoveRotation(newRotation);
    }
}