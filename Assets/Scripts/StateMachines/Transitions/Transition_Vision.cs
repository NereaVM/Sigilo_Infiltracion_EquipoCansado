using System;
using System.Linq;
using UnityEngine;

public class Transition_Vision : Transition
{
    VisionSense visionSense;
    public override bool IsTriggered()
    {
        visionSense = stateMachine.visionSense;
        if (visionSense == null) {
            Debug.LogError($"VisionSense not found. This transition requires a VisionSense component.[TransitionVision][IsTriggered]");
            return false;
        }

        return visionSense.hasSeenEntity;
    }

    public override void TransitionAction()
    {
        visionSense.ResetVision();
    }
}