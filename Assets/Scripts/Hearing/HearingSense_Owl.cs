using UnityEngine;

public class HearingSense_Owl : HearingSense
{
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
}