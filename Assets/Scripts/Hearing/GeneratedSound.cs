using System.Collections.Generic;
using UnityEngine;

public class GeneratedSound : HearingSystem
{
    [SerializeField] private SoundType soundType;
    [SerializeField] private float duration; // Duration of the sound before dissapearing. -1 means infinite duration
    [SerializeField] private bool onlyHeardOncePerEntity = false; // If true, the sound will only be heard once by each entity. If false, it can be heard multiple times.
    public HearingSense soundGenerator; // Reference to the HearingSense component that generated this sound for ignoring the sound generator itself. 
    private List<HearingSense> affectedEntities;
    void OnEnable()
    {
        // Destroy after a set time
        if (duration >= 0) Destroy(gameObject, duration);
    }

    void OnTriggerEnter(Collider other)
    {
        HearingSense hearingSense = other.GetComponent<HearingSense>();
        if (hearingSense == null || hearingSense == soundGenerator) return;
        if (onlyHeardOncePerEntity && HasHeardAlready(hearingSense)) return;

        hearingSense.HearSound(soundType, transform.position);
        Debug.Log("Tried to hear sound. [GeneratedSound][OnTriggerEnter]");
        
        if (onlyHeardOncePerEntity) affectedEntities.Add(hearingSense);
    }

    bool HasHeardAlready(HearingSense hearingSense)
    {
        foreach (var entity in affectedEntities)
        {
            if (entity == hearingSense)
            {
                Debug.Log("Entity already affected by this sound. [GeneratedSound][CheckIfAlreadyHeard]");
                return true;
            }
        }

        return false;
    }

    void OnDrawGizmos()
    {
        SphereCollider sphereCollider = GetComponentInChildren<SphereCollider>();
        if (sphereCollider == null)
        {
            return;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sphereCollider.radius * sphereCollider.transform.lossyScale.x);
    }
}