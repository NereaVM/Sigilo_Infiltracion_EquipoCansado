
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

        Debug.Log(
            "[DEFEAT] El jugador ha tocado al Jabalí Alfa."
        );

        TriggerDefeat();
    }

    private void TriggerDefeat()
    {
        if (defeated)
            return;

        defeated = true;

        // Desactivar la posibilidad de pausar.
        if (pauseMenu != null)
        {
            pauseMenu.SetPauseAllowed(false);
        }

        // Mostrar pantalla de derrota.
        if (defeatPanel != null)
        {
            defeatPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning(
                "[DEFEAT] No hay panel de derrota asignado."
            );
        }

        // Detener el juego.
        Time.timeScale = 0f;

        // Reiniciar después del tiempo indicado.
        StartCoroutine(RestartAfterDelay());
    }

    private IEnumerator RestartAfterDelay()
    {
        // Realtime permite esperar aunque
        // Time.timeScale sea 0.
        yield return new WaitForSecondsRealtime(
            restartDelay
        );

        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
}
