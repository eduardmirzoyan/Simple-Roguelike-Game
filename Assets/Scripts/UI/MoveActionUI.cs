using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MoveActionUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image highlightImage;
    [SerializeField] private CanvasGroup tooltipGroup;

    [Header("Debug")]
    [SerializeField, ReadOnly] private PlayerData playerData;

    public void Initialize(PlayerData playerData)
    {
        this.playerData = playerData;

        GameEvents.instance.OnTurnStart += OnTurnStart;
        GameEvents.instance.OnAttackSelect += OnAttackSelect;
        GameEvents.instance.OnAttackCancel += OnAttackCancel;
        GameEvents.instance.OnActionStart += OnActionStart;
    }

    private void OnDestroy()
    {
        GameEvents.instance.OnTurnStart -= OnTurnStart;
        GameEvents.instance.OnAttackSelect -= OnAttackSelect;
        GameEvents.instance.OnAttackCancel -= OnAttackCancel;
        GameEvents.instance.OnActionStart -= OnActionStart;
    }

    private void OnTurnStart(EntityData data)
    {
        if (playerData == data)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }

    private void OnAttackSelect(WeaponData data)
    {
        canvasGroup.alpha = 0.6f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    private void OnAttackCancel(WeaponData data)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    private void OnActionStart()
    {
        canvasGroup.alpha = 0.6f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltipGroup.alpha = 1f;
        highlightImage.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipGroup.alpha = 0f;
        highlightImage.enabled = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Does nothing.
    }
}
