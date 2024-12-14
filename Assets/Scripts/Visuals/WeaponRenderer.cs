using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRenderer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private WeaponTooltipUI weaponTooltipUI;

    [Header("Settings")]
    [SerializeField] private float popoutDistance;
    [SerializeField] private float popoutDuration;
    [SerializeField] private float hoverDistance;
    [SerializeField] private float hoverDuration;

    [Header("Data")]
    [SerializeField, ReadOnly] private WeaponData weaponData;

    public void Initialize(WeaponData weaponData)
    {
        this.weaponData = weaponData;
        spriteRenderer.sprite = weaponData.sprite;

        gameObject.name = $"{weaponData.name} Drop";

        StartCoroutine(Animation());

        GameEvents.instance.OnWeaponDrop += OnDrop;
        GameEvents.instance.OnWeaponPickup += OnPickup;
        GameEvents.instance.OnTileEnter += OnTileEnter;
    }
    private void OnDestroy()
    {
        GameEvents.instance.OnWeaponDrop -= OnDrop;
        GameEvents.instance.OnWeaponPickup -= OnPickup;
        GameEvents.instance.OnTileEnter -= OnTileEnter;
    }

    private void OnDrop(int _, WeaponData weaponData, TileData __)
    {
        if (this.weaponData == weaponData)
        {
            string instructions = $"[Shift] + [F] or [G] to EQUIP";
            weaponTooltipUI.Show(weaponData, instructions);
        }
        else
        {
            weaponTooltipUI.Hide();
        }
    }


    private void OnPickup(int _, WeaponData weaponData, TileData __)
    {
        if (this.weaponData == weaponData)
        {
            Destroy(gameObject);
        }
    }

    private void OnTileEnter(EntityData entityData, TileData tileData)
    {
        // If player entered the tile this weapon is on
        if (tileData.weapon == weaponData && entityData is PlayerData)
        {
            string instructions = $"[Shift] + [F] or [G] to EQUIP";
            weaponTooltipUI.Show(weaponData, instructions);
        }
        else
        {
            weaponTooltipUI.Hide();
        }
    }

    private IEnumerator Animation()
    {
        // Pop out
        LeanTween.moveLocalY(spriteRenderer.gameObject, popoutDistance, popoutDuration / 2).setEaseOutQuad();
        yield return new WaitForSeconds(popoutDuration / 2);
        LeanTween.moveLocalY(spriteRenderer.gameObject, 0f, popoutDuration / 2).setEaseInQuad();
        yield return new WaitForSeconds(popoutDuration / 2);
        // Hover
        LeanTween.moveLocalY(spriteRenderer.gameObject, hoverDistance, hoverDuration).setEaseInOutSine().setLoopPingPong();
    }
}
