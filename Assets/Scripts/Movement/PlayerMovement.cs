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

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerEnergy = GetComponent<PlayerEnergy>();

        // El juego no tiene movimiento vertical.
        rb.useGravity = false;

        rb.constraints =
            RigidbodyConstraints.FreezePositionY |
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationZ;
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
        // Evita aumentar la velocidad al moverse en diagonal.
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

        // Energía 100% -> velocidad máxima.
        // Energía 0%   -> velocidad mínima.
        float currentSpeed = Mathf.Lerp(
            minMoveSpeed,
            maxMoveSpeed,
            playerEnergy.NormalizedEnergy
        );

        Vector3 movement =
            direction * currentSpeed * Time.fixedDeltaTime;

        rb.MovePosition(rb.position + movement);

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