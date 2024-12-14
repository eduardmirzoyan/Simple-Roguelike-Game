using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class WeaponActionUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image weaponImage;
    [SerializeField] private Image highlightImage;
    [SerializeField] private Image outlineImage;
    [SerializeField] private TextMeshProUGUI hotKeyLabel;
    [SerializeField] private GameObject tooltipPrefab;

    [Header("Cooldown")]
    [SerializeField] private Image crossIcon;
    [SerializeField] private TextMeshProUGUI cooldownLabel;

    [Header("Settings")]
    [SerializeField] private Vector3 tooltipOffset;
    [SerializeField] private int weaponIndex;

    [Header("Debug")]
    [SerializeField, ReadOnly] private WeaponTooltipUI weaponTooltip;
    [SerializeField, ReadOnly] private EntityData entityData;
    [SerializeField, ReadOnly] private WeaponData weaponData;
    [SerializeField, ReadOnly] private KeyCode hotkey;

    public void Initialize(EntityData entityData, int weaponIndex)
    {
        this.entityData = entityData;
        this.weaponIndex = weaponIndex;
        hotkey = PlayerMananger.weaponKeyCodeMap[weaponIndex];
        hotKeyLabel.text = $"[{hotkey}]";

        SetWeapon(entityData.weapons[weaponIndex]);

        // Get top-left corner
        Vector3[] corners = new Vector3[4];
        GetComponent<RectTransform>().GetWorldCorners(corners);

        Vector3 worldPosition = corners[1] + tooltipOffset;
        weaponTooltip = Instantiate(tooltipPrefab, worldPosition, Quaternion.identity, transform).GetComponent<WeaponTooltipUI>();

        GameEvents.instance.OnTurnStart += OnTurnStart;
        GameEvents.instance.OnAttackSelect += OnAttackSelect;
        GameEvents.instance.OnAttackCancel += OnAttackCancel;
        GameEvents.instance.OnActionStart += OnActionStart;

        GameEvents.instance.OnWeaponPickup += OnPickup;
        GameEvents.instance.OnWeaponDrop += OnDrop;
        GameEvents.instance.OnTurnEnd += OnTurnEnd;
    }

    private void OnDestroy()
    {
        GameEvents.instance.OnTurnStart -= OnTurnStart;
        GameEvents.instance.OnAttackSelect -= OnAttackSelect;
        GameEvents.instance.OnAttackCancel -= OnAttackCancel;
        GameEvents.instance.OnActionStart -= OnActionStart;

        GameEvents.instance.OnWeaponPickup -= OnPickup;
        GameEvents.instance.OnWeaponDrop -= OnDrop;
        GameEvents.instance.OnTurnEnd -= OnTurnEnd;
    }

    private void OnPickup(int index, WeaponData weaponData, TileData tileData)
    {
        if (weaponIndex == index)
        {
            SetWeapon(weaponData);
        }
    }

    private void OnDrop(int index, WeaponData weaponData, TileData tileData)
    {
        if (weaponIndex == index)
        {
            SetWeapon(null);
        }
    }

    private void OnTurnStart(EntityData entityData)
    {
        if (this.entityData == entityData)
        {
            // Set active
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }

    private void OnAttackSelect(WeaponData weaponData)
    {
        // Set outlined or not
        if (this.weaponData == weaponData)
        {
            outlineImage.enabled = true;
        }
        else
        {
            outlineImage.enabled = false;
        }
    }

    private void OnAttackCancel(WeaponData _)
    {
        // Set normal
        outlineImage.enabled = false;
    }

    private void OnActionStart()
    {
        canvasGroup.alpha = 0.6f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }


    private void SetWeapon(WeaponData weaponData)
    {
        if (weaponData != null)
        {
            weaponImage.sprite = weaponData.sprite;
            weaponImage.enabled = true;
        }
        else
        {
            weaponImage.enabled = false;
        }

        this.weaponData = weaponData;
    }

    private void OnTurnEnd(EntityData entityData)
    {
        if (this.entityData == entityData)
        {
            UpdateCooldown();

            // Set Faded
            canvasGroup.alpha = 0.6f;
            outlineImage.enabled = false;
        }
    }

    private void UpdateCooldown()
    {
        if (weaponData == null)
            return;

        if (weaponData.cooldownTimer > 0)
        {
            crossIcon.enabled = true;
            cooldownLabel.enabled = true;
            cooldownLabel.text = "" + weaponData.cooldownTimer;
        }
        else
        {
            crossIcon.enabled = false;
            cooldownLabel.enabled = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        var weapon = entityData.weapons[weaponIndex];
        if (weapon != null)
        {
            string instructions = $"[Shift] + [{hotkey}] to DROP";
            weaponTooltip.Show(entityData.weapons[weaponIndex], instructions, true);
        }
        highlightImage.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        weaponTooltip.Hide(true);
        highlightImage.enabled = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            GameManager.instance.EntitySelectAttack(entityData, weaponIndex);
    }
}
