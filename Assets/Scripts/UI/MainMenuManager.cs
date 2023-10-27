using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager instance;
    private void Awake()
    {
        // Singleton Logic
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void Start()
    {
        // Open scene in center
        TransitionManager.instance.Initialize(transform);
        TransitionManager.instance.OpenScene();
    }

    public void StartGame()
    {
        // Debug
        print("Start Game");

        // Create new player
        DataManager.instance.CreateNewPlayer();

        // Load next scene
        TransitionManager.instance.LoadNextScene();
    }

    public void HowToPlay()
    {
        // Debug
        print("Open Help Screen");
    }

    public void QuitGame()
    {
        // Debug
        print("Quit Game");

        // Close application
        Application.Quit();
    }

}
