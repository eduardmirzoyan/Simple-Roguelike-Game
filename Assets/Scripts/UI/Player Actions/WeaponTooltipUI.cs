using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WeaponTooltipUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI weaponName;
    [SerializeField] private TextMeshProUGUI weaponType;
    [SerializeField] private TextMeshProUGUI weaponDamage;
    [SerializeField] private TextMeshProUGUI weaponRange;
    [SerializeField] private TextMeshProUGUI weaponDescription;
    [SerializeField] private TextMeshProUGUI weaponInstructions;

    [Header("Settings")]
    [SerializeField] private float fadeDuration;

    private void Awake()
    {
        Hide();
    }

    public void Show(WeaponData weaponData, string instructions = "", bool instant = false)
    {
        if (weaponData == null)
            throw new System.Exception("Tooltip attempted to display null data.");

        weaponName.text = weaponData.name;
        weaponType.text = weaponData.range > 1 ? "Ranged Weapon" : "Melee Weapon";
        weaponDamage.text = $"{weaponData.damageRange.x}-{weaponData.damageRange.y}";
        weaponRange.text = $"{weaponData.range}";
        weaponDescription.text = weaponData.description;
        weaponInstructions.text = instructions;

        // Update content to fit in tooltip
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());

        StopAllCoroutines();
        if (instant)
            canvasGroup.alpha = 1f;
        else
            StartCoroutine(FadeIn(fadeDuration));
    }

    public void Hide(bool instant = false)
    {
        if (canvasGroup.alpha < 0.1f)
            return;

        StopAllCoroutines();
        if (instant)
            canvasGroup.alpha = 0f;
        else
            StartCoroutine(FadeOut(fadeDuration));
    }

    private IEnumerator FadeIn(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / duration);

            elapsed += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    private IEnumerator FadeOut(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / duration);

            elapsed += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0f;
    }
}
