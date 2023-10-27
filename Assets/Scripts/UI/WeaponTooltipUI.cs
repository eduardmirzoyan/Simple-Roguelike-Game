using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WeaponTooltipUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI weaponName;
    [SerializeField] private TextMeshProUGUI weaponType;
    [SerializeField] private TextMeshProUGUI weaponDamage;
    [SerializeField] private TextMeshProUGUI weaponRange;
    [SerializeField] private TextMeshProUGUI weaponCooldown;
    [SerializeField] private TextMeshProUGUI weaponDescription;
    [SerializeField] private TextMeshProUGUI weaponInstructions;

    private void Start()
    {
        Hide();
    }

    public void Show(WeaponData weaponData, string instructions)
    {
        if (weaponData == null)
            return;

        weaponName.text = weaponData.name;
        weaponType.text = weaponData.range > 1 ? "Ranged Weapon" : "Melee Weapon";
        weaponDamage.text = weaponData.damage + "";
        weaponRange.text = weaponData.range + "";
        weaponCooldown.text = weaponData.cooldown - 1 + "";
        weaponDescription.text = weaponData.description;
        weaponInstructions.text = instructions;

        canvasGroup.alpha = 1f;
    }

    public void Hide()
    {
        canvasGroup.alpha = 0f;
    }
}
