using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon/Basic")]
public class SwordWeapon : WeaponData
{
    public override void Attack(EntityData holderData, TileData targetData)
    {
        // If enemy exists at this tile
        if (targetData.entityData != null)
        {
            GameManager.instance.EntityTakeDamage(targetData.entityData, damage, holderData);
        }
    }
}
