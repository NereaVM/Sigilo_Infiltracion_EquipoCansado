
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerEnergy))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float maxMoveSpeed = 4f;
    [SerializeField] private float minMoveSpeed = 1.5f;
    [SerializeField] private float rotationSpeed = 10f;

    private Rigidbody rb;
    private PlayerEnergy playerEnergy;

    private Vector2 movementInput;

    public bool IsMoving =>
        movementInput.sqrMagnitude > 0.001f;

    public float CurrentMoveSpeed
    {
        get
        {
            if (!IsMoving || playerEnergy == null)
                return 0f;

            return Mathf.Lerp(
                minMoveSpeed,
                maxMoveSpeed,
                playerEnergy.NormalizedEnergy
            );
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerEnergy = GetComponent<PlayerEnergy>();

        // El jugador utiliza físicas para colisionar.
        rb.isKinematic = false;

        // No utilizamos gravedad ni movimiento vertical.
        rb.useGravity = false;

        // Bloqueamos el movimiento vertical y los giros
        // producidos por las físicas.
        rb.constraints =
            RigidbodyConstraints.FreezePositionY |
            RigidbodyConstraints.FreezeRotation;
    }

    private void Update()
    {
        if (IsMoving)
        {
            playerEnergy.ConsumeEnergy(Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

    public void SetMovementInput(Vector2 input)
    {
        // Evita aumentar la velocidad en diagonal.
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
        {
            // Detener el movimiento cuando no hay input.
            rb.linearVelocity = Vector3.zero;
            return;
        }

        // La velocidad depende de la energía.
        float currentSpeed = Mathf.Lerp(
            minMoveSpeed,
            maxMoveSpeed,
            playerEnergy.NormalizedEnergy
        );

        // Movimiento mediante velocidad física.
        rb.linearVelocity = direction * currentSpeed;

        // Girar hacia la dirección del movimiento.
        Quaternion targetRotation =
            Quaternion.LookRotation(direction, Vector3.up);

        Quaternion newRotation =
            Quaternion.Slerp(
                rb.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime
            );

        // El giro lo controla el código, no los impactos.
        rb.MoveRotation(newRotation);
    }
}
