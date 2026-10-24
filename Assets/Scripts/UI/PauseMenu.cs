using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject pauseButton;

    private bool isPaused = false;
    private bool pauseAllowed = true;

    private void Awake()
    {
        Time.timeScale = 1f;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (pauseButton != null)
            pauseButton.SetActive(true);
    }

    private void Update()
    {
        if (!pauseAllowed)
            return;

        if (Keyboard.current != null &&
            Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        if (!pauseAllowed)
            return;

        isPaused = true;

        if (pausePanel != null)
            pausePanel.SetActive(true);

        if (pauseButton != null)
            pauseButton.SetActive(false);

        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        if (!pauseAllowed)
            return;

        isPaused = false;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (pauseButton != null)
            pauseButton.SetActive(true);

        Time.timeScale = 1f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    public void ExitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();
    }

    public void SetPauseAllowed(bool value)
    {
        pauseAllowed = value;

        if (!value)
        {
            isPaused = false;

            if (pausePanel != null)
                pausePanel.SetActive(false);

            if (pauseButton != null)
                pauseButton.SetActive(false);
        }
        else
        {
            if (pauseButton != null)
                pauseButton.SetActive(true);
        }
    }
}