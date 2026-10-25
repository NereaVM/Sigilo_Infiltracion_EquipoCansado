using UnityEngine;

public class Transition_CrocodileReturnReached : Transition
{
    private CrocodileReturnState returnState;

    public override bool IsTriggered()
    {
        if (returnState == null)
        {
            returnState =
                GetComponent<CrocodileReturnState>();
        }

        return returnState != null &&
            returnState.HasReachedReturnPosition();
    }
}
