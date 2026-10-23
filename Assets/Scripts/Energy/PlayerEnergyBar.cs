using UnityEngine;
using UnityEngine.UI;

public class PlayerEnergyBar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerEnergy playerEnergy;
    [SerializeField] private Image energyFill;

    private void Update()
    {
        if (playerEnergy == null || energyFill == null)
            return;

        energyFill.fillAmount =
            playerEnergy.NormalizedEnergy;
    }
}