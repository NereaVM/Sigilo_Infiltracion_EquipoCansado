using System;
using System.Linq;
using UnityEngine;

public class Transition_Hearing : Transition
{
    [SerializeField] private bool onlyOwlAlert;

    HearingSense hearingSense;
    public override bool IsTriggered()
    {
        hearingSense = stateMachine.hearingSense;
        if (hearingSense == null) {
            Debug.LogError($"HearingSense not found. This transition requires a HearingSense component.[TransitionHearing][IsTriggered]");
            return false;
        }

        return hearingSense.hasHeardSound &&
               (!onlyOwlAlert ||
                     hearingSense.lastSoundType == HearingSystem.SoundType.OwlAlert);
    }

    public override void TransitionAction()
    {
        hearingSense.ResetHearing();
    }
}