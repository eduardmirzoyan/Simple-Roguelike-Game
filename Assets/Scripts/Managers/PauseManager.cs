using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Data")]
    [SerializeField] private KeyCode pauseKey = KeyCode.Tab;
    [SerializeField] private bool isPaused;

    public bool IsPaused
    {
        get { return isPaused; }
    }

    public static PauseManager instance;
    private void Awake()
    {
        // Singleton logic
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;

        canvasGroup = GetComponentInChildren<CanvasGroup>();
        isPaused = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            if (!isPaused) Pause();
            else Resume();
        }
    }

    public void Pause()
    {
        isPaused = true;

        // Stop time
        Time.timeScale = 0f;

        // Make menu visible
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void Resume()
    {
        isPaused = false;

        // Start time
        Time.timeScale = 1f;

        // Make menu invisible
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void Restart()
    {
        Resume();

        // Start a new run
        TransitionManager.instance.ReloadScene();
    }

    public void MainMenu()
    {
        // Resume first
        Resume();

        // Tell game to return to main menu
        TransitionManager.instance.LoadMainMenuScene();
    }

    public void Quit()
    {
        Debug.Log("Player quit game.");
        Application.Quit();
    }
}
