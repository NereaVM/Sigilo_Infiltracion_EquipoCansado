using UnityEngine;

public class BoarPatrolState : State
{
    public override void OnEntry()
    {
        Debug.Log("Boar entered Patrol state.");
    }

    public override void OnUpdate()
    {
        // Movement is handled by MovementSystemController.
    }

    public override void OnExit()
    {
        Debug.Log("Boar exited Patrol state.");
    }
}