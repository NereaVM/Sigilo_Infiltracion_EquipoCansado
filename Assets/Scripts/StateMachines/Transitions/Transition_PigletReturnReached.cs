using UnityEngine;

public class Transition_PigletReturnReached : Transition
{
    private PigletReturnState returnState;

    public override bool IsTriggered()
    {
        if (returnState == null)
        {
            returnState = GetComponent<PigletReturnState>();
        }

        if (returnState == null) return false;

        return returnState.HasReachedPatrolRoute();
    }
}
