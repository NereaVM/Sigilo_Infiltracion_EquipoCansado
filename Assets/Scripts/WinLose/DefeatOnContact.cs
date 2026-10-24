using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DefeatOnContact : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject defeatPanel;
    [SerializeField] private PauseMenu pauseMenu;

    [Header("Restart")]
    [SerializeField] private float restartDelay = 5f;

    private bool defeated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (defeated)
            return;

        if (!other.CompareTag("Player"))
            return;

        defeated = true;

        if (pauseMenu != null)
            pauseMenu.SetPauseAllowed(false);

        if (defeatPanel != null)
            defeatPanel.SetActive(true);

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