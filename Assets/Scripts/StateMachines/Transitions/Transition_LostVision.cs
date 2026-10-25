using UnityEngine;

public class Transition_LostVision : Transition
{
    [SerializeField] private float lostSightDelay = 1.5f;

    private float timeWithoutSeeing = 0f;

    public override bool IsTriggered()
    {
        VisionSense visionSense = stateMachine.visionSense;

        if (visionSense == null)
        {
            Debug.LogError("VisionSense not found.");
            return false;
        }

        if (visionSense.hasSeenPlayer)
        {
            timeWithoutSeeing = 0f;
            return false;
        }

        timeWithoutSeeing += Time.deltaTime;

        return timeWithoutSeeing >= lostSightDelay;
    }

    public override void TransitionAction()
    {
        timeWithoutSeeing = 0f;
    }
}