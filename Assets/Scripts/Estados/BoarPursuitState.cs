using UnityEngine;

public class BoarPursuitState : State
{
    [SerializeField] private Transform target;

    public override void OnEntry()
    {
        Debug.Log("Boar entered Pursuit state.");

        foreach (MovementScript movementScript in movementScripts)
        {
            movementScript.targetTransform = target;
        }
    }

    public override void OnUpdate()
    {
        // Movement is handled by MovementSystemController.
    }

    public override void OnExit()
    {
        Debug.Log("Boar exited Pursuit state.");
    }
}