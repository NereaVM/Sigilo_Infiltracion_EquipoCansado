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
    private StateMachine stateMachine;
    public void SetStateMachine(StateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }
    private HearingSense hearingSense;
    private VisionSense visionSense;

    private void Awake()
    {
        // Disable all of the movement scripts on this state, as they should only be enabled when the state is active.
        foreach (var movementScript in movementScripts)
        {
            movementScript.enabled = false;
        }
    }
    /// <summary>
    ///     Actions to perform when transitioning from the previous state to this one.
    /// </summary>
    public abstract void OnEntry();
    public void _OnEntry()
    {
        foreach (Transition transition in transitions)
        {
            transition.SetStateMachine(stateMachine);
        }
    }
    
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
    ///     MovementScripts that will control the movement of the NPC while in this state.
    /// </summary>
    public MovementScript[] movementScripts;
    
}