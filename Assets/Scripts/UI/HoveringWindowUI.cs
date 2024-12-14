using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoveringWindowUI : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float displacement = 5f;
    [SerializeField] private float period = 2f;

    private void Start()
    {
        LeanTween.moveLocalY(gameObject, displacement, period).setEase(LeanTweenType.easeInOutSine).setLoopPingPong();
    }
}
