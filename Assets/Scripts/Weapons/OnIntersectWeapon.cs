using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon/On Intersect")]
public class OnIntersectWeapon : WeaponData
{
    public override int CalculateDamage(EntityData holderData, EntityData targetData)
    {
        // TODO

        return damage;
    }
}
