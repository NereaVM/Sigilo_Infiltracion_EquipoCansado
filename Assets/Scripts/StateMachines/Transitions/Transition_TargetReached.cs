using UnityEngine;

public class Transition_TargetReached : Transition
{
    [SerializeField] private Transform target;
    [SerializeField] private float targetRadius = 1.2f;

    public override bool IsTriggered()
    {
        if (target == null || stateMachine == null)
        {
            return false;
        }

        float distance = Vector3.Distance(
            stateMachine.transform.position,
            target.position
        );

        return distance <= targetRadius;
    }
}