using UnityEngine;

public class WinPoint : MonoBehaviour
{
    [SerializeField] private GameObject victoryPanel;

    private bool gameWon = false;

    private void OnTriggerEnter(Collider other)
    {
        if (gameWon)
            return;

        if (other.CompareTag("Player"))
        {
            gameWon = true;

            if (victoryPanel != null)
                victoryPanel.SetActive(true);

            Time.timeScale = 0f;
        }
    }
}