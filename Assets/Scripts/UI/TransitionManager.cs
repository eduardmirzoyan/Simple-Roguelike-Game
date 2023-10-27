using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private TransitionUI transitionUI;

    [Header("Data")]
    [SerializeField] private float transitionTime = 1f;
    private Coroutine coroutine;

    public static TransitionManager instance;
    private void Awake()
    {
        // Singleton logic
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;

        transitionUI = GetComponent<TransitionUI>();
    }

    public void Initialize(Transform target)
    {
        transitionUI.Initialize(target);
    }

    public void OpenScene()
    {
        transitionUI.OpenBlackScreen();

        if (GetSceneIndex() == 0)
        {
            // Play title music
            AudioManager.instance.PlayOST("Background " + 0);
        }
        else
        {
            // Play background music
            AudioManager.instance.PlayOST("Background " + 1);
        }
    }

    public void LoadNextScene()
    {
        // Stop any background music
        if (GetSceneIndex() == 0)
        {
            // Play title music
            AudioManager.instance.StopOST("Background " + 0);
        }
        else
        {
            // Play background music
            AudioManager.instance.StopOST("Background " + 1);
        }

        // Stop any transition if one was happening
        if (coroutine != null) StopCoroutine(coroutine);

        // Transition to next scene
        coroutine = StartCoroutine(LoadScene(SceneManager.GetActiveScene().buildIndex + 1));
    }

    public void LoadSelectedScene(int buildIndex)
    {
        // Stop any background music
        if (GetSceneIndex() == 0)
        {
            // Play title music
            AudioManager.instance.StopOST("Background " + 0);
        }
        else
        {
            // Play background music
            AudioManager.instance.StopOST("Background " + 1);
        }

        // Stop any transition if one was happening
        if (coroutine != null) StopCoroutine(coroutine);

        // Transition to next scene
        coroutine = StartCoroutine(LoadScene(buildIndex));
    }

    public void ReloadScene()
    {
        // Stop any background music
        if (GetSceneIndex() == 0)
        {
            // Play title music
            AudioManager.instance.StopOST("Background " + 0);
        }
        else
        {
            // Play background music
            AudioManager.instance.StopOST("Background " + 1);
        }

        // Stop any transition if one was happening
        if (coroutine != null) StopCoroutine(coroutine);

        // Transition to same scene
        coroutine = StartCoroutine(LoadScene(SceneManager.GetActiveScene().buildIndex));
    }

    public void LoadMainMenuScene()
    {
        // Stop any background music
        if (GetSceneIndex() == 0)
        {
            // Play title music
            AudioManager.instance.StopOST("Background " + 0);
        }
        else
        {
            // Play background music
            AudioManager.instance.StopOST("Background " + 1);
        }

        // Stop any transition if one was happening
        if (coroutine != null) StopCoroutine(coroutine);

        // Transition to main menu, scene 0
        coroutine = StartCoroutine(LoadScene(0));
    }

    private IEnumerator LoadScene(int index)
    {
        transitionUI.CloseBlackScreen();

        // Wait
        yield return new WaitForSeconds(transitionTime);

        // Check if next scene exists
        int maxCount = SceneManager.sceneCountInBuildSettings;
        if (index < maxCount)
        {
            // Load scene
            SceneManager.LoadScene(index);
        }
        else
        {
            // Debug
            print("Could not find scene " + index);

            // Load scene 0
            SceneManager.LoadScene(0);
        }
    }

    private int GetSceneIndex()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }
}