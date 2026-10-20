using UnityEngine;

public class Transition_OwlRelocate : Transition
{
    [Header("Relocation Interval")]
    [SerializeField] private bool randomInterval = true;

    [SerializeField] private float fixedInterval = 12f;

    [SerializeField] private float minRandomInterval = 8f;
    [SerializeField] private float maxRandomInterval = 15f;

    private float timer;
    private float currentInterval;

    private void OnEnable()
    {
        ResetTimer();
    }

    public override bool IsTriggered()
    {
        timer += Time.deltaTime;
        return timer >= currentInterval;
    }

    public override void TransitionAction()
    {
        ResetTimer();
    }

    public void ResetTimer()
    {
        timer = 0f;

        if (randomInterval)
        {
            float min = Mathf.Min(minRandomInterval, maxRandomInterval);
            float max = Mathf.Max(minRandomInterval, maxRandomInterval);

            currentInterval = Random.Range(min, max);
        }
        else
        {
            currentInterval = fixedInterval;
        }

        currentInterval = Mathf.Max(0.1f, currentInterval);
    }
}
