using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinPoint : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private PauseMenu pauseMenu;

    [Header("Restart")]
    [SerializeField] private float restartDelay = 5f;

    private bool gameWon = false;

    private void OnTriggerEnter(Collider other)
    {
        if (gameWon)
            return;

        if (!other.CompareTag("Player"))
            return;

        gameWon = true;

        if (pauseMenu != null)
            pauseMenu.SetPauseAllowed(false);

        if (victoryPanel != null)
            victoryPanel.SetActive(true);

        Time.timeScale = 0f;

        StartCoroutine(RestartAfterDelay());
    }

    private IEnumerator RestartAfterDelay()
    {
        yield return new WaitForSecondsRealtime(restartDelay);

        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
}