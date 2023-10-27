using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResourcebarUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject barObject;
    [SerializeField] private TextMeshProUGUI valueLabel;
    [SerializeField] private Image fillImage;
    [SerializeField] private Image backgroundFillImage;

    [Header("Settings")]
    [SerializeField] private float rate;

    public void Initialize(int current, int max)
    {
        if (max == 0)
        {
            barObject.SetActive(false);
            return;
        }
        else
        {
            barObject.SetActive(true);
        }

        LeanTween.cancel(barObject);
        valueLabel.text = current + " / " + max;

        float fill = (float)current / max;
        fillImage.fillAmount = fill;
        backgroundFillImage.fillAmount = fill;
    }

    public void UpdateValue(int current, int max)
    {
        if (max == 0)
        {
            barObject.SetActive(false);
            return;
        }
        else
        {
            barObject.SetActive(true);
        }

        // Update text immediately
        valueLabel.text = current + " / " + max;

        // Update bar over time
        float oldFill = fillImage.fillAmount;
        float newFill = (float)current / max;

        LeanTween.cancel(barObject);
        if (newFill < oldFill) // If decreased
        {
            fillImage.fillAmount = newFill;
            LeanTween.value(barObject, oldFill, newFill, rate).setOnUpdate((float fill) =>
            {
                backgroundFillImage.fillAmount = fill;
            });
        }
        else if (newFill > oldFill) // If increased
        {
            backgroundFillImage.fillAmount = newFill;
            LeanTween.value(barObject, oldFill, newFill, rate).setOnUpdate((float fill) =>
            {
                fillImage.fillAmount = fill;
            });
        }
        else
        {
            // Do nothing
        }
    }
}
