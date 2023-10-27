using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon/Bonus When Has Guard")]
public class GuardDamageWeapon : WeaponData
{
    // + 1 damage while target has any posture
    [SerializeField] private int damageBonus;

    public override void Attack(EntityData holderData, TileData targetData)
    {
        if (targetData.entityData != null)
        {
            if (targetData.entityData.currentPosture > 0)
            {
                GameManager.instance.EntityTakeDamage(targetData.entityData, damage + damageBonus, holderData);
            }
            else
            {
                GameManager.instance.EntityTakeDamage(targetData.entityData, damage, holderData);
            }
        }
    }
}
