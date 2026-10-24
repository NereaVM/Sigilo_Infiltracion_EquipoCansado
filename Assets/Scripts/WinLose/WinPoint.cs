using UnityEngine;

public class WinPoint : MonoBehaviour
{
    [Header("Menus")]
    [SerializeField] private WinMenu winMenu;
    [SerializeField] private PauseMenu pauseMenu;

    private bool gameWon = false;

    private void OnTriggerEnter(Collider other)
    {
        if (gameWon)
            return;

        if (!other.CompareTag("Player"))
            return;

        gameWon = true;

        // Impide abrir el menú de pausa después de ganar
        if (pauseMenu != null)
            pauseMenu.SetPauseAllowed(false);

        // Muestra la pantalla de victoria
        if (winMenu != null)
            winMenu.ShowWinMenu();
    }
}