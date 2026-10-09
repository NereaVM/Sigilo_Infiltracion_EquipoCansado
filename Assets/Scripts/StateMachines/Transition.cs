using UnityEngine;

/// <summary>
///     Represents a transition between two states in a state machine.
/// </summary>
/// <remarks>
///     Transitions should ideally be as generic as possible, as they are meant to refer to changes 
///     between the base states, and not the specific per-NPC states based on them.
/// </remarks>
public abstract class Transition : MonoBehaviour
{
    public StateMachine stateMachine;
    public void SetStateMachine(StateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    /// <summary>
    ///     Check if the transition condition is triggered.
    /// </summary>
    /// <returns>
    ///     True if the transition condition is triggered; otherwise, false.
    /// </returns>
    public abstract bool IsTriggered();

    /// <summary>
    ///     The target state to transition to when the condition is triggered.
    /// </summary>
    public State targetState;
    public State GetTargetState()
    {
        return targetState;
    }

    /// <summary>
    ///     The action to perform when the transition is applied.
    /// </summary>
    /// <remarks>
    ///    Ideally, most actions should be performed either in OnExit() of the current state or OnEnter() of the target state, <br/>
    ///    but this method is provided for cases where the action is specific to the transition itself.
    /// </remarks>
    public virtual void TransitionAction(){}
    public void _TransitionAction()
    {
        targetState.SetStateMachine(stateMachine);
        TransitionAction();
    }
}