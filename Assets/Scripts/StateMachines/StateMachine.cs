using System;
using UnityEngine;

[RequireComponent(typeof(MovementSystemController))]
public class StateMachine : MonoBehaviour
{
    [Tooltip("The GameObject that holds the states for this state machine.")]
    [SerializeField] private State initialState;
    public HearingSense hearingSense { get; private set; }
    public VisionSense visionSense { get; private set; }
    private MovementSystemController movementSystemController;
    private State currentState;

    private void Awake()
    {   
        movementSystemController = GetComponent<MovementSystemController>();
        // Ensure we are looking for the Hearing and Vision components in this gameObject,
        // if they are not found, they should be manually added in the inspector or assumed to be kept null.
        _ = hearingSense == null ? hearingSense = GetComponent<HearingSense>() : hearingSense = hearingSense;
        _ = visionSense == null ? visionSense = GetComponent<VisionSense>() : visionSense = visionSense;
        visionSense = GetComponent<VisionSense>();
    }

    private void Start()
    {
        // Assign the initial state to the current state and call its OnEnter method
        currentState = initialState;
        currentState.SetStateMachine(this);
        if (currentState == null) throw new Exception("Initial state is not set. Please assign an initial state in the inspector.");
        currentState.OnEntry();
    }

    /// <summary>
    ///     Applies the transition to the target state, calling the exit method of the current state, <br/>
    ///     the transition action, and the enter method of the target state.
    /// </summary>
    private void ApplyTransition(Transition transition)
    {
        currentState.OnExit();
        transition._TransitionAction();
        currentState = transition.GetTargetState();
        movementSystemController?.ChangeToMovementScripts(currentState.movementScripts);        
        currentState._OnEntry();
    }

    private void CheckTransitions()
    {
        // Check if any of the transitions from the current state are triggered.
        foreach (var transition in currentState.GetTransitions())
        {
            // If the condition for a transition is met, apply the transition and break out of the loop.
            // Applies only the first transition that is triggered, as transitions are prioritized by their order in the array in State.
            if (transition.IsTriggered())
            {
                ApplyTransition(transition);
                break;
            }
        }
    }

    private void Update()
    {
        if (currentState == null) throw new Exception("Current state is null. Ensure that the initial state is set and that transitions are properly configured.");

        // Check if any of the transitions from the current state are triggered.
        CheckTransitions();

        // If no transitions are triggered, continue updating the current state.
        currentState.OnUpdate();
    }


}