using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon/Self Damage")]
public class SelfDamageWeapon : WeaponData
{
    [SerializeField, ReadOnly] private int bonusDamage;
    [SerializeField] private int bonusCap;

    private EntityData holder;

    public override int GetTotalDamage(EntityData holderData, EntityData targetData)
    {
        return damage + bonusDamage;
    }

    public override void Initialize(EntityData holder)
    {
        this.holder = holder;
        bonusDamage = 0;

        // Sub to onhit event
    }

    public override void Unintialize()
    {
        holder = null;
        bonusDamage = 0;

        // Unsub to onhit event
    }

    private void OnDamageTaken(EntityData entityData)
    {
        bonusDamage = Mathf.Min(bonusDamage + 1, bonusCap);
    }
}
