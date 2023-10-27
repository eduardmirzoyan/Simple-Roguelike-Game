using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamagePreviewUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI damageLabel;

    [Header("Settings")]
    [SerializeField] private Vector3 offset;

    public static DamagePreviewUI instance;
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

    public void Show(int damage, Vector3 position)
    {
        damageLabel.text = $"{damage} Damage";
        transform.position = position + offset;
        canvasGroup.alpha = 1f;
    }

    public void Hide()
    {
        canvasGroup.alpha = 0f;
    }
}
