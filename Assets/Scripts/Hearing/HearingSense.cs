using UnityEngine;

public abstract class HearingSense : HearingSystem
{

    MonoBehaviour parentScript;

    public virtual int HearSound(SoundType soundType)
    {

        int priority = GetPriority(soundType);

        switch (soundType)
        {
            case SoundType.CommonSound:
                Debug.Log("Common sound heard. [HearingSense][HearSound]");
                break;
            case SoundType.PigletAlert:
                Debug.Log("Piglet alert sound heard. [HearingSense][HearSound]");
                break;
            case SoundType.OwlAlert:
                Debug.Log("Owl alert sound heard. [HearingSense][HearSound]");
                break;
            default:
                Debug.Log("Unknown sound type. [HearingSense][HearSound]");
                break;
        }

        return priority;

    }

    public abstract int GetPriority(SoundType soundType);


    public virtual void SetParentScript(MonoBehaviour script)
    {
        parentScript = script;
    }
}