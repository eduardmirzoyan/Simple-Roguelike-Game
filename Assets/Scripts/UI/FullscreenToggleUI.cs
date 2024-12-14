using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FullscreenToggleUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Image iconImage;
    [SerializeField] private Sprite fullscreenSprite;
    [SerializeField] private Sprite windowSprite;

    [Header("Debug")]
    [SerializeField] private bool isFullScreen;

    private void Start()
    {
        isFullScreen = Screen.fullScreen;
        iconImage.sprite = isFullScreen ? windowSprite : fullscreenSprite;
    }

    public void Toggle()
    {
        // If we are full
        if (isFullScreen)
        {
            // Go to windowed
            Screen.fullScreen = false;
            iconImage.sprite = fullscreenSprite;
        }
        else
        {
            // Go to full
            Screen.fullScreen = true;
            iconImage.sprite = windowSprite;
        }

        isFullScreen = !isFullScreen;
    }
}
