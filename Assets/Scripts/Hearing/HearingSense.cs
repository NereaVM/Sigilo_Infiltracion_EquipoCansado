using UnityEngine;

public abstract class HearingSense : HearingSystem
{
    public bool hasHeardSound { get; set;}
    public Vector3 lastSoundPosition { get; set;}
    public SoundType lastSoundType { get; set;}

    public virtual void HearSound(SoundType soundType, Vector3 soundPosition)
    {
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

        if (!hasHeardSound)
        {
            hasHeardSound = true;
            lastSoundPosition = soundPosition;
            lastSoundType = soundType;
        }

    }

    public abstract int GetPriority(SoundType soundType);

    public void ResetHearing()
    {
        hasHeardSound = false;
    }
}