using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDropUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject tooltipPrefab;

    [Header("Settings")]
    [SerializeField] private Vector3 tooltipOffset;

    [Header("Debug")]
    [SerializeField, ReadOnly] private WeaponTooltipUI weaponTooltip;

    public static WeaponDropUI instance;
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

    private void Start()
    {
        // Get bottom-right corner
        Vector3[] corners = new Vector3[4];
        GetComponent<RectTransform>().GetWorldCorners(corners);

        Vector3 worldPosition = corners[3] + tooltipOffset;
        weaponTooltip = Instantiate(tooltipPrefab, worldPosition, Quaternion.identity, transform).GetComponent<WeaponTooltipUI>();

        GameEvents.instance.onWeaponPickup += OnPickup;
        GameEvents.instance.onWeaponDrop += OnDrop;
    }

    private void OnDestroy()
    {
        GameEvents.instance.onWeaponPickup -= OnPickup;
        GameEvents.instance.onWeaponDrop -= OnDrop;
    }

    private void OnPickup(int index, WeaponData weaponData, TileData tileData)
    {
        // TODO?
    }

    private void OnDrop(int index, WeaponData weaponData, TileData tileData)
    {
        // TODO?
    }

    public void Show(WeaponData weapon)
    {
        string instructions = $"[Shift] + [F] or [G] to EQUIP";
        weaponTooltip.Show(weapon, instructions);
    }

    public void Hide()
    {
        weaponTooltip.Hide();
    }
}
