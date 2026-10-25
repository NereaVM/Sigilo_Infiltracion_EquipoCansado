using UnityEngine;

public class Transition_BoarInvestigationFinished : Transition
{
    private BoarInvestigateSoundState investigateState;

    public override bool IsTriggered()
    {
        if (investigateState == null)
            investigateState = GetComponent<BoarInvestigateSoundState>();

        return investigateState != null &&
               investigateState.IsInspectionComplete;
    }
}