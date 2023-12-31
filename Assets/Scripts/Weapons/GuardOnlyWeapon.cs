using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon/Guard Only")]
public class GuardOnlyWeapon : WeaponData
{
    public override int CalculateDamage(EntityData holderData, EntityData targetData)
    {
        return Mathf.Min(targetData.currentPosture, damage);
    }
}
