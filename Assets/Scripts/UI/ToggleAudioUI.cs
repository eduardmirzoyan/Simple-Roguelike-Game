using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class ToggleAudioUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Image slashImage;

    [Header("Data")]
    [SerializeField] private string parameterName;

    [Header("Debugging")]
    [SerializeField, ReadOnly] private bool isOn;

    private void Awake()
    {
        audioMixer.GetFloat(parameterName, out float value);
        if (value < 0)
        {
            slashImage.enabled = true;
            isOn = false;
        }
        else
        {
            slashImage.enabled = false;
            isOn = true;
        }
    }

    public void Toggle()
    {
        if (isOn)
        {
            audioMixer.SetFloat(parameterName, -80f);
            slashImage.enabled = true;
            isOn = false;
        }
        else
        {
            audioMixer.SetFloat(parameterName, 0f);
            slashImage.enabled = false;
            isOn = true;
        }
    }
}
