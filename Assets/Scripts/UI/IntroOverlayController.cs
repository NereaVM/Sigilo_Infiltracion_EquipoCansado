using System.Collections;
using UnityEngine;

public class IntroOverlayController : MonoBehaviour
{
    // Se mantiene al recargar la escena.
    // Al cerrar y volver a abrir el juego vuelve a false.
    private static bool hasPlayedThisSession = false;

    [Header("References")]
    [SerializeField] private CanvasGroup canvasGroup;

    [SerializeField]
    private RectTransform sneakyMonkeyLogo;

    [SerializeField]
    private RectTransform teamLogo;

    [SerializeField]
    private RectTransform ujiLogo;

    [SerializeField]
    private PauseMenu pauseMenu;

    [SerializeField]
    private GameObject energyBar;

    [SerializeField]
    private GameObject blackBackground;

    [Header("Timing")]
    [SerializeField] private float holdDuration = 0.25f;
    [SerializeField] private float fadeDuration = 0.75f;

    [Header("Scale")]
    [SerializeField] private float startScale = 1.15f;
    [SerializeField] private float endScale = 0.85f;

    private Vector3 sneakyOriginalScale;
    private Vector3 teamOriginalScale;
    private Vector3 ujiOriginalScale;

    private void Start()
    {
        if (hasPlayedThisSession)
        {
            if (pauseMenu != null)
                pauseMenu.SetPauseAllowed(true);

            if (energyBar != null)
                energyBar.SetActive(true);

            if (blackBackground != null)
                blackBackground.SetActive(false);

            gameObject.SetActive(false);
            return;
        }

        hasPlayedThisSession = true;

        if (pauseMenu != null)
            pauseMenu.SetPauseAllowed(false);

        if (energyBar != null)
            energyBar.SetActive(false);

        // Mostramos el fondo negro durante la intro.
        if (blackBackground != null)
            blackBackground.SetActive(true);

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (sneakyMonkeyLogo != null)
        {
            sneakyOriginalScale = sneakyMonkeyLogo.localScale;

            sneakyMonkeyLogo.localScale =
                sneakyOriginalScale * startScale;
        }

        if (teamLogo != null)
        {
            teamOriginalScale = teamLogo.localScale;

            teamLogo.localScale =
                teamOriginalScale * startScale;
        }

        if (ujiLogo != null)
        {
            ujiOriginalScale = ujiLogo.localScale;

            ujiLogo.localScale =
                ujiOriginalScale * startScale;
        }

        if (canvasGroup != null)
            canvasGroup.alpha = 1f;

        StartCoroutine(PlayIntro());
    }

    private IEnumerator PlayIntro()
    {
        yield return new WaitForSecondsRealtime(
            holdDuration
        );

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;

            float t =
                Mathf.Clamp01(
                    timer / fadeDuration
                );

            t = Mathf.SmoothStep(
                0f,
                1f,
                t
            );

            if (canvasGroup != null)
                canvasGroup.alpha = 1f - t;

            if (sneakyMonkeyLogo != null)
            {
                sneakyMonkeyLogo.localScale =
                    Vector3.Lerp(
                        sneakyOriginalScale * startScale,
                        sneakyOriginalScale * endScale,
                        t
                    );
            }

            if (teamLogo != null)
            {
                teamLogo.localScale =
                    Vector3.Lerp(
                        teamOriginalScale * startScale,
                        teamOriginalScale * endScale,
                        t
                    );
            }

            if (ujiLogo != null)
            {
                ujiLogo.localScale =
                    Vector3.Lerp(
                        ujiOriginalScale * startScale,
                        ujiOriginalScale * endScale,
                        t
                    );
            }

            yield return null;
        }

        if (canvasGroup != null)
            canvasGroup.alpha = 0f;

        if (pauseMenu != null)
            pauseMenu.SetPauseAllowed(true);

        if (energyBar != null)
            energyBar.SetActive(true);

        if (blackBackground != null)
            blackBackground.SetActive(false);

        gameObject.SetActive(false);
    }
}