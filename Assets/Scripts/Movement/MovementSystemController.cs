using UnityEngine;

public class MovementSystemController : MonoBehaviour
{
    [Tooltip("The movement scripts that will be used by the state machine.")]
    [SerializeField] MovementScript[] movementScripts;

    public void SetUpMovementScripts(State[] states)
    {
        int count = 0;
        foreach (var state in states)
        {
            if (state.movementScript != null)
            {
                
                count++;

            }
        }
        movementScripts = new MovementScript[];
        for (int i = 0; i < states.Length; i++)
        {
            movementScripts[i] = states[i].movementScript;
        }
    }
}