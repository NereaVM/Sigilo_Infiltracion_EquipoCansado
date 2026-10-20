using UnityEngine;

public class Transition_Timer : Transition
{
    [SerializeField] private float duration = 3f;

    private float timer = 0f;

    public override bool IsTriggered()
    {
        timer += Time.deltaTime;
        return timer >= duration;
    }

    public override void TransitionAction()
    {
        ResetTimer();
    }

    public void ResetTimer()
    {
        timer = 0f;
    }
}