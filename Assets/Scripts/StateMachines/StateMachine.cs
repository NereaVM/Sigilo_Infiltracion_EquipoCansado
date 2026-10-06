using System;
using UnityEngine;

[RequireComponent(typeof(MovementSystemController))]
public class StateMachine : MonoBehaviour
{
    [Tooltip("The GameObject that holds the states for this state machine.")]
    [SerializeField] private GameObject stateHolder;
    [SerializeField] private State initialState;
    private MovementSystemController movementSystemController;
    private State currentState;

    private void Awake()
    {
        // Assume the GameObject that holds the states is self.
        // We allow for it to be a different GameObject in case the states are organized in a different hierarchy.
        if (stateHolder == null) stateHolder = this.gameObject;
        
        movementSystemController = GetComponent<MovementSystemController>();
        
    }

    private void Start()
    {
        // Assign the initial state to the current state and call its OnEnter method
        currentState = initialState;
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
        transition.TransitionAction();
        currentState = transition.GetTargetState();
        movementSystemController.ChangeToMovementScripts(currentState.movementScripts);        
        currentState.OnEntry();
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