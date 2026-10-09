using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     The MovementSystemController is responsible for managing the movement scripts <br/>
///     that control the movement of the NPCs in the game. <br/>
///     <br/>
///     It is designed to be used in conjunction with the StateMachine, <br/>
///     allowing different movement scripts to be assigned to different states of the NPCs.
/// </summary>
/// <remarks>
///     The MovementSystemController is a component that should be attached to the same GameObject as the StateMachine. <br/>
///     It provides a centralized way to manage the movement scripts, <br/>
///     lifting the responsibility of managing movement scripts from individual states.
/// </remarks>
public class MovementSystemController : MonoBehaviour
{
    public Transform currentTargetTransform;
    public Vector3 currentTargetPosition;
    
    private List<MovementScript> activeMovementScripts = new List<MovementScript>();

    private void DisableMovementScript(MovementScript movementScript)
    {

        if (activeMovementScripts.Contains(movementScript))
        {
            activeMovementScripts.Remove(movementScript);
        }
        //movementScript.enabled = false;
    }

    private void EnableMovementScript(MovementScript movementScript)
    {
        if (!activeMovementScripts.Contains(movementScript))
        {
            activeMovementScripts.Add(movementScript);
        }
        //movementScript.enabled = true;
    }

    /// <summary>
    ///    Receives an array of MovmentScripts and enables them, <br/>
    ///    while disabling any other MovementScripts that were previously active.
    /// </summary>
    /// <param name="movementScripts"></param>
    public void ChangeToMovementScripts(MovementScript[] movementScripts)
    {
        // Disables the scripts not in the new array
        foreach (var activeScript in new List<MovementScript>(activeMovementScripts))
        {
            bool isInNewList = false;
            foreach (MovementScript newScript in movementScripts)        
            {
                if (activeScript == newScript) isInNewList = true;
            }
            if (!isInNewList) DisableMovementScript(activeScript);
        }

        // Enables the scripts in the new array that aren't already active.
        foreach (var newScript in movementScripts)
        {
            if (!activeMovementScripts.Contains(newScript)) EnableMovementScript(newScript);
        }
    }

    public void SetNewTarget(Vector3 newTarget)
    {
        currentTargetPosition = newTarget;
    }

    public void SetNewTarget(Transform newTarget)
    {
        currentTargetTransform = newTarget;
    }

    /// <summary>
    ///    Calls the OnUpdate() method of all active MovementScripts <br/>
    ///    and updates their target. 
    /// </summary>
    public virtual void UpdateActiveMovementScripts()
    {
        foreach (var movementScript in activeMovementScripts)
        {
            if (currentTargetTransform != null)
            {
                currentTargetPosition = currentTargetTransform.position;
            }
            movementScript.target = currentTargetPosition;
            movementScript.OnUpdate();
        }
    }

    private void FixedUpdate()
    {
        UpdateActiveMovementScripts();
    }

}
