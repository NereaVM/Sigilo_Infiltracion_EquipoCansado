
using System.Collections.Generic;
using UnityEngine;

public class GeneratedSound : HearingSystem
{
    [SerializeField] private SoundType soundType;
    [SerializeField] private float duration;
    [SerializeField] private bool onlyHeardOncePerEntity = false;

    public HearingSense soundGenerator;

    private List<HearingSense> affectedEntities = new List<HearingSense>();

    private void OnEnable()
    {
        // Destroy after a set time.
        // -1 means infinite duration.
        if (duration >= 0)
        {
            Destroy(gameObject, duration);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        HearingSense hearingSense = other.GetComponentInParent<HearingSense>();

        if (hearingSense == null || hearingSense == soundGenerator)
            return;

        if (onlyHeardOncePerEntity && HasHeardAlready(hearingSense))
            return;

        hearingSense.HearSound(soundType, transform.position);

        // Debug para comprobar qué animal recibe el sonido.
        Debug.Log(
            $"[GeneratedSound] {hearingSense.gameObject.name} " +
            $"ha recibido una señal de tipo {soundType} " +
            $"desde {transform.position}"
        );

        if (onlyHeardOncePerEntity)
        {
            affectedEntities.Add(hearingSense);
        }
    }

    private bool HasHeardAlready(HearingSense hearingSense)
    {
        foreach (var entity in affectedEntities)
        {
            if (entity == hearingSense)
            {
                Debug.Log(
                    "Entity already affected by this sound. " +
                    "[GeneratedSound][HasHeardAlready]"
                );

                return true;
            }
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        SphereCollider sphereCollider = GetComponentInChildren<SphereCollider>();

        if (sphereCollider == null)
            return;

        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            transform.position,
            sphereCollider.radius * sphereCollider.transform.lossyScale.x
        );
    }
}
