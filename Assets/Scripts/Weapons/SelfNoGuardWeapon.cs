using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon/Self No Guard")]
public class SelfNoGuardWeapon : WeaponData
{
    [SerializeField] private int damageBonus;

    public override int GetTotalDamage(EntityData holderData, EntityData targetData)
    {
        return holderData.currentPosture == 0 ? damage + damageBonus : damage;
    }
}
