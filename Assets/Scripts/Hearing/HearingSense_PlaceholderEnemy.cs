using UnityEngine;

public class HearingSense_PlaceholderEnemy : HearingSense
{

    public bool hasHeardSound {get; private set;} = false ;
    public Vector3 lastSoundPosition {get; private set;}
    public SoundType lastSoundType {get; private set;}

    public override int GetPriority(SoundType soundType)
    {
        switch (soundType)
        {
            case SoundType.OwlAlert:
                return 3; 
            case SoundType.PigletAlert:
                return 2; 
            case SoundType.CommonSound:
                return 1; 
            default:
                throw new System.ArgumentOutOfRangeException(nameof(soundType), soundType, null);
        }
    }

    public override int HearSound(SoundType soundType, Vector3 soundPosition)
    {
        int priority = GetPriority(soundType);

        if (!hasHeardSound)
        {
            hasHeardSound = true;
            lastSoundPosition = soundPosition;
            lastSoundType = soundType;
        }

        return priority;

    }

    public void ResetHearing()
    {
        hasHeardSound = false;
    }
}
