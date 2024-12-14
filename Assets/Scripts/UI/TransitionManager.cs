using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private RectTransform blackScreenTransform;

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
    }

    public int GetSceneIndex()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }

    public void OpenScene()
    {
        // Play animation
        LeanTween.moveLocalX(blackScreenTransform.gameObject, -blackScreenTransform.rect.width, transitionTime);

        // Play background music
        AudioManager.instance.PlayOST("Background " + GetSceneIndex());
    }

    public void LoadNextScene()
    {
        // Stop any background music
        AudioManager.instance.StopOST("Background " + GetSceneIndex());

        // Stop any transition if one was happening
        if (coroutine != null) StopCoroutine(coroutine);

        // Transition to next scene
        coroutine = StartCoroutine(LoadScene(SceneManager.GetActiveScene().buildIndex + 1));
    }

    public void LoadPreviousScene()
    {
        // Stop any background music
        AudioManager.instance.StopOST("Background " + GetSceneIndex());

        // Stop any transition if one was happening
        if (coroutine != null) StopCoroutine(coroutine);

        // Transition to next scene
        coroutine = StartCoroutine(LoadScene(SceneManager.GetActiveScene().buildIndex - 1));
    }

    public void ReloadScene()
    {
        // Stop any transition if one was happening
        if (coroutine != null) StopCoroutine(coroutine);

        // Transition to same scene
        coroutine = StartCoroutine(LoadScene(SceneManager.GetActiveScene().buildIndex));
    }

    public void LoadMainMenuScene()
    {
        // Stop any background music
        AudioManager.instance.StopOST("Background " + GetSceneIndex());

        // Stop any transition if one was happening
        if (coroutine != null) StopCoroutine(coroutine);

        // Transition to main menu, scene 0
        coroutine = StartCoroutine(LoadScene(0));
    }

    private IEnumerator LoadScene(int index)
    {
        // Play animation
        LeanTween.cancel(blackScreenTransform.gameObject);
        LeanTween.moveLocalX(blackScreenTransform.gameObject, blackScreenTransform.rect.width, 0f);
        LeanTween.moveLocalX(blackScreenTransform.gameObject, 0, transitionTime);

        // Wait
        yield return new WaitForSeconds(transitionTime);

        // Load scene
        SceneManager.LoadScene(index);
    }
}