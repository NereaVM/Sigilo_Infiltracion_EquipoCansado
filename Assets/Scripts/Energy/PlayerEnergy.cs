using UnityEngine;

public class PlayerEnergy : MonoBehaviour
{
    [Header("Energy")]
    [SerializeField] private float maxEnergy = 100f;
    [SerializeField] private float energyConsumptionPerSecond = 10f;

    private float currentEnergy;

    public float CurrentEnergy => currentEnergy;
    public float MaxEnergy => maxEnergy;

    // 0 = sin energía
    // 1 = energía máxima
    public float NormalizedEnergy =>
        maxEnergy > 0f ? currentEnergy / maxEnergy : 0f;

    private void Awake()
    {
        currentEnergy = maxEnergy;
    }

    /// <summary>
    /// Consume energía mientras el jugador se está moviendo.
    /// </summary>
    public void ConsumeEnergy(float deltaTime)
    {
        currentEnergy -= energyConsumptionPerSecond * deltaTime;

        currentEnergy = Mathf.Clamp(
            currentEnergy,
            0f,
            maxEnergy
        );
    }

    /// <summary>
    /// Recupera toda la energía.
    /// Se utilizará posteriormente al recoger un plátano.
    /// </summary>
    public void RestoreEnergy()
    {
        currentEnergy = maxEnergy;
    }
}