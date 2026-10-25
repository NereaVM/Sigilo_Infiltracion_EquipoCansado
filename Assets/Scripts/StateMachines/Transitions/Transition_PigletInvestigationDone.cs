using UnityEngine;

public class Transition_PigletInvestigationDone : Transition
{
    private PigletInvestigateState investigateState;

    public override bool IsTriggered()
    {
        if (investigateState == null)
        {
            investigateState = GetComponent<PigletInvestigateState>();
        }

        if (investigateState == null) return false;

        return investigateState.ShouldReturn();
    }
}
