using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon/Consecutive Miss")]
public class OnIntersectWeapon : WeaponData
{
    public override int GetTotalDamage(EntityData holderData, EntityData targetData)
    {
        // TODO

        return damage;
    }
}
