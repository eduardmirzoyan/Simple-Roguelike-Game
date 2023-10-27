using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip audioClip;

    [Range(0, 1f)]
    public float volume = 1f;

    [Range(-1f, 1f)]
    public float pitch = 1f;

    public bool loop = false;

    public bool ignorePause = false;

    [HideInInspector]
    public AudioSource audioSource;

    public AudioMixerGroup audioMixerGroup;
}
