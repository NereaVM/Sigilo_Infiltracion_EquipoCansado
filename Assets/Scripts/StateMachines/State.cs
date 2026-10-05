using UnityEngine;

/// <summary>
///     Represents a state in a state machine.
/// </summary>
/// <remarks>
///     The intended workflow for States is to define the minimum conceptual states of the game AI <br/>
///     and implement them as base states inheriting State, <br/>
///     then create per-NPC specific versions of those States that override the OnUpdate() method.
///     <br/><br/>
///     State does not implement its own Update, it only defines behaviours that should be controlled by the State Machine.
/// </remarks>
public abstract class State : MonoBehaviour
{
    /// <summary>
    ///     Actions to perform when transitioning from the previous state to this one.
    /// </summary>
    public abstract void OnEnter();
    /// <summary>
    ///     Actions to perform on the Update loop.
    /// </summary>
    /// <remarks>
    ///     The call to OnUpdate() should happen on the State Machine's loop.
    /// </remarks>
    public abstract void OnUpdate();

    /// <summary>
    ///     Actions to perform when transitioning from this state to another one.
    /// </summary>
    public abstract void OnExit();

    /// <summary>
    ///    The transitions that can be triggered from this state.
    /// </summary>
    /// <remarks>
    ///     Ensure the higher priority transitions are listed first, <br/>
    ///    as the State Machine will apply the first transition that finds as triggered.
    /// </remarks>
    public Transition[] transitions;
    public Transition[] GetTransitions()
    {
        return transitions;
    }


    /// <summary>
    ///     MovementScript that will control the movement of the NPC while in this state.
    /// </summary>
    /// <remarks>
    ///     In the current implementation, only one MovementScript can be assigned to a state, <br/>
    ///     as the Align behaviour is expected to be used in every state.
    /// </remarks>
    public MovementScript movementScript;
    public MovementScript GetMovementScript()
    {
        return movementScript;
    }
    public MovementSystemController movementSystemController;
    
}