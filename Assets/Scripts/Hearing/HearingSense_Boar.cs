using UnityEngine;

public class HearingSense_Boar : HearingSense
{
    public override int GetPriority(SoundType soundType)
        {
            switch (soundType)
            {
                case SoundType.PigletAlert:
                    return 3; 
                case SoundType.OwlAlert:
                    return 2; 
                case SoundType.CommonSound:
                    return 1; 
                default:
                    throw new System.ArgumentOutOfRangeException(nameof(soundType), soundType, null);
            }
        }
}