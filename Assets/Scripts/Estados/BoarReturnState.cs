using UnityEngine;

public class BoarReturnState : State
{
    [SerializeField] private Transform returnTarget;

    private MovementSystemController movementSystemController;

    public override void OnEntry()
    {
        Debug.Log("Boar entered Return state.");

        movementSystemController =
            GetComponentInParent<MovementSystemController>();

        if (movementSystemController != null)
        {
            movementSystemController.SetNewTarget(returnTarget);
        }

        foreach (MovementScript movementScript in movementScripts)
        {
            movementScript.targetTransform = returnTarget;
        }
    }

    public override void OnUpdate()
    {
        // Movement is handled by MovementSystemController.
    }

    public override void OnExit()
    {
        Debug.Log("Boar exited Return state.");
    }
}