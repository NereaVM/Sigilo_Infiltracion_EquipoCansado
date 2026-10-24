using System.Collections.Generic;
using UnityEngine;

public class PlayerBushStealth : MonoBehaviour
{
    private int bushesInside = 0;

    private readonly HashSet<VisionSense> allowedObservers =
        new HashSet<VisionSense>();

    public bool IsHidden => bushesInside > 0;

    public void EnterBush()
    {
        bushesInside++;

        // Only capture who was already pursuing when entering
        // the first bush.
        if (bushesInside != 1)
            return;

        allowedObservers.Clear();

        VisionSense[] visionSenses =
            FindObjectsByType<VisionSense>(
                FindObjectsSortMode.None
            );

        foreach (VisionSense visionSense in visionSenses)
        {
            if (visionSense == null)
                continue;

            StateMachine stateMachine =
                visionSense.GetComponent<StateMachine>();

            if (stateMachine == null)
                continue;

            if (IsPursuitState(stateMachine.CurrentState))
            {
                allowedObservers.Add(visionSense);
            }
        }
    }

    public void ExitBush()
    {
        bushesInside =
            Mathf.Max(0, bushesInside - 1);

        if (bushesInside == 0)
        {
            allowedObservers.Clear();
        }
    }

    public bool CanBeSeenBy(VisionSense observer)
    {
        if (!IsHidden)
            return true;

        if (observer == null)
            return false;

        // Only NPCs that were already pursuing when the player
        // entered the bush may continue seeing the player.
        if (!allowedObservers.Contains(observer))
            return false;

        StateMachine stateMachine =
            observer.GetComponent<StateMachine>();

        if (stateMachine == null)
            return false;

        // If that NPC stops pursuing while the player is still
        // hidden, it loses permission to reacquire through the bush.
        return IsPursuitState(stateMachine.CurrentState);
    }

    private bool IsPursuitState(State state)
    {
        return state is BoarPursuitState ||
               state is OwlAlertState;
    }
}
