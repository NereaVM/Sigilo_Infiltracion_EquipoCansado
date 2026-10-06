using UnityEngine;

public class DefeatOnContact : MonoBehaviour
{
    [SerializeField] private GameObject defeatPanel;

    private bool defeated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (defeated)
            return;

        if (other.CompareTag("Player"))
        {
            defeated = true;

            if (defeatPanel != null)
                defeatPanel.SetActive(true);

            Time.timeScale = 0f;
        }
    }
}