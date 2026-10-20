using UnityEngine;

public class Transition_OwlRelocateFinished : Transition
{
    [SerializeField] private OwlRelocateState relocateState;

    public override bool IsTriggered()
    {
        return relocateState != null && relocateState.HasFinished;
    }

    public override void TransitionAction()
    {
    }
}
