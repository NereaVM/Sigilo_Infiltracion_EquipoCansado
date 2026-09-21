using UnityEngine;

public class VisibleEntity : MonoBehaviour
{
    [SerializeField] private bool canBeSeen = true;
    [SerializeField] private Transform visionTarget;

    public bool CanBeSeen => canBeSeen;

    public Vector3 GetVisionPosition()
    {
        if (visionTarget != null)
        {
            return visionTarget.position;
        }

        return transform.position;
    }

    public void SetCanBeSeen(bool value)
    {
        canBeSeen = value;
    }
}