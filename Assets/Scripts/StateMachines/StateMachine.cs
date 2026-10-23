using System;
using UnityEngine;

[RequireComponent(typeof(MovementSystemController))]
public class StateMachine : MonoBehaviour
{
    [Tooltip("The GameObject that holds the states for this state machine.")]
    [SerializeField] private State initialState;

    [Header("Debug")]
    [SerializeField] private string currentStateName;

    public HearingSense hearingSense { get; private set; }
    public VisionSense visionSense { get; private set; }

    private MovementSystemController movementSystemController;
    private State currentState;

    // NEREA:
    // Allows other systems to read the current state
    // without being able to modify it.
    public State CurrentState => currentState;

    private void Awake()
    {
        movementSystemController = GetComponent<MovementSystemController>();

        // Ensure we are looking for the Hearing and Vision components
        // in this GameObject.
        if (hearingSense == null)
        {
            hearingSense = GetComponent<HearingSense>();
        }

        if (visionSense == null)
        {
            visionSense = GetComponent<VisionSense>();
        }
    }

    private void Start()
    {
        // Assign the initial state.
        currentState = initialState;

        // Check before using it.
        if (currentState == null)
        {
            throw new Exception(
                "Initial state is not set. Please assign an initial state in the inspector."
            );
        }

        // Give the state access to this StateMachine.
        currentState.SetStateMachine(this);

        // Configure the movement scripts of the initial state.
        movementSystemController.ChangeToMovementScripts(
            currentState.movementScripts
        );

        // _OnEntry also calls the specific OnEntry().
        currentState._OnEntry();

        UpdateCurrentStateDebug();
    }

    /// <summary>
    /// Applies the transition to the target state.
    /// </summary>
    private void ApplyTransition(Transition transition)
    {
        State previousState = currentState;

        // Exit the current state.
        currentState.OnExit();

        // Execute the transition-specific action.
        transition._TransitionAction();

        // Change to the target state.
        currentState = transition.GetTargetState();

        if (currentState == null)
        {
            throw new Exception(
                "Transition target state is null."
            );
        }

        // Configure the movement scripts required by the new state.
        movementSystemController.ChangeToMovementScripts(
            currentState.movementScripts
        );

        // _OnEntry also calls the specific OnEntry().
        currentState._OnEntry();

        UpdateCurrentStateDebug();

        Debug.Log(
            $"[STATE MACHINE] {gameObject.name}: " +
            $"{previousState.gameObject.name} -> {currentState.gameObject.name}"
        );
    }

    private void CheckTransitions()
    {
        // Check if any of the transitions from the current state are triggered.
        foreach (var transition in currentState.GetTransitions())
        {
            if (transition.IsTriggered())
            {
                ApplyTransition(transition);
                break;
            }
        }
    }

    private void Update()
    {
        if (currentState == null)
        {
            throw new Exception(
                "Current state is null. Ensure that the initial state is set " +
                "and that transitions are properly configured."
            );
        }

        CheckTransitions();

        currentState.OnUpdate();
    }

    private void UpdateCurrentStateDebug()
    {
        currentStateName =
            currentState != null
            ? currentState.gameObject.name
            : "None";

        Debug.Log(
            $"[STATE MACHINE] {gameObject.name} current state: {currentStateName}"
        );
    }
}