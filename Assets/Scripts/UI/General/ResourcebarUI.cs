using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResourcebarUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI valueLabel;
    [SerializeField] private Image fillImage;

    [Header("Settings")]
    [SerializeField] private float flashDuration;

    public void UpdateValue(int current, int max)
    {
        float ratio = (float)current / max;

        // Update values immediately
        valueLabel.text = $"{current} / {max}";
        fillImage.fillAmount = ratio;

        return;

        // Flash change
        StartCoroutine(Flash(fillImage, flashDuration));
    }

    private IEnumerator Flash(Image image, float duration)
    {
        var originalColor = image.color;

        image.color = Color.white;
        yield return new WaitForSeconds(duration);
        image.color = originalColor;
    }
}
