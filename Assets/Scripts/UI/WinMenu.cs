using UnityEngine;
using UnityEngine.SceneManagement;

public class WinMenu : MonoBehaviour
{
    [SerializeField] private GameObject victoryPanel;

    private void Start()
    {
        victoryPanel.SetActive(false);
    }

    public void ShowWinMenu()
    {
        victoryPanel.SetActive(true);
        Time.timeScale = 0f;
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

        Debug.Log("Exit game");
    }
}