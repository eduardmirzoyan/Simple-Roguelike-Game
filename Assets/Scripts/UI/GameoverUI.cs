using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameoverUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private CanvasGroup bgCanvasGroup;
    [SerializeField] private CanvasGroup windowCanvasGroup;
    [SerializeField] private TextMeshProUGUI scoreLabel;

    [Header("Settings")]
    [SerializeField] private float bgFadeDuration;
    [SerializeField] private float delayDuration;
    [SerializeField] private float windowFadeDuration;

    private void Start()
    {
        Hide();

        GameEvents.instance.onGameOver += Show;
    }

    private void OnDestroy()
    {
        GameEvents.instance.onGameOver -= Show;
    }

    public void Hide()
    {
        bgCanvasGroup.alpha = 0f;
        bgCanvasGroup.interactable = false;
        bgCanvasGroup.blocksRaycasts = false;

        windowCanvasGroup.alpha = 0f;
        windowCanvasGroup.interactable = false;
        windowCanvasGroup.blocksRaycasts = false;
    }

    public void Show(int score)
    {
        scoreLabel.text = $"Score: {score}";

        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        // Set both invisible
        bgCanvasGroup.alpha = 0f;
        windowCanvasGroup.alpha = 0f;

        // Stop interaction
        bgCanvasGroup.interactable = true;
        bgCanvasGroup.blocksRaycasts = true;

        // Wait
        yield return new WaitForSeconds(delayDuration);

        // Fade in background first
        float elapsed = 0f;
        while (elapsed < bgFadeDuration)
        {
            bgCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / bgFadeDuration);

            elapsed += Time.deltaTime;
            yield return null;
        }
        bgCanvasGroup.alpha = 1f;

        // Wait
        yield return new WaitForSeconds(delayDuration);

        // Allow interaction with menu
        windowCanvasGroup.interactable = true;
        windowCanvasGroup.blocksRaycasts = true;

        // Then fade in window
        elapsed = 0f;
        while (elapsed < windowFadeDuration)
        {
            windowCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / windowFadeDuration);

            elapsed += Time.deltaTime;
            yield return null;
        }
        windowCanvasGroup.alpha = 1f;
    }

    public void Restart()
    {
        TransitionManager.instance.ReloadScene();
    }

    public void MainMenu()
    {
        // Tell game to return to main menu
        TransitionManager.instance.LoadMainMenuScene();
    }
}
