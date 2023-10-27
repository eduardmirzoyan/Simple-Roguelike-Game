using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRenderer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;

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

        gameObject.name = weaponData.name + " Drop";

        StartCoroutine(Animation());

        GameEvents.instance.onWeaponPickup += OnPickup;
    }

    private void OnDestroy()
    {
        GameEvents.instance.onWeaponPickup -= OnPickup;
    }

    private void OnPickup(int _, WeaponData weaponData, TileData __)
    {
        print("HIT!");

        if (this.weaponData == weaponData)
        {
            Destroy(gameObject);
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
