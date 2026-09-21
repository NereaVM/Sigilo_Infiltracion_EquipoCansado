using UnityEngine;
using UnityEngine.InputSystem;

public class MovementTestInput : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;

    private void Awake()
    {
        if (playerMovement == null)
            playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        Vector2 input = Vector2.zero;

        if (Keyboard.current == null)
            return;

        if (Keyboard.current.wKey.isPressed)
            input.y += 1f;

        if (Keyboard.current.sKey.isPressed)
            input.y -= 1f;

        if (Keyboard.current.aKey.isPressed)
            input.x -= 1f;

        if (Keyboard.current.dKey.isPressed)
            input.x += 1f;

        playerMovement.SetMovementInput(input);
    }
}