using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon/Consecutive Miss")]
public class ConsecutiveMissWeapon : WeaponData
{
    [SerializeField, ReadOnly] private int bonusDamage;
    [SerializeField] private int damageCap;

    private EntityData holder;

    public override int GetTotalDamage(EntityData holderData, EntityData targetData)
    {
        return damage + bonusDamage;
    }

    public override void Initialize(EntityData holder)
    {
        this.holder = holder;
        damage = 0;

        GameEvents.instance.onEntityAttackTile += OnMiss;
    }

    public override void Unintialize()
    {
        holder = null;
        damage = 0;

        GameEvents.instance.onEntityAttackTile -= OnMiss;
    }

    private void OnMiss(EntityData entityData, WeaponData weaponData, TileData tileData)
    {
        if (this == weaponData)
        {
            if (tileData.entityData != null)
                damage = 0;
            else
                damage = Mathf.Min(damage + 1, damageCap);
        }
    }
}
