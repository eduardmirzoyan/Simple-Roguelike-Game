using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon/Bonus When On Cooldown")]
public class BonusWhenCooldownWeapon : WeaponData
{
    [SerializeField] private int damageBonus;

    public override void Attack(EntityData holderData, TileData targetData)
    {
        if (targetData.entityData != null)
        {
            bool applyBonus = false;
            foreach (var weapon in holderData.weapons)
            {
                if (weapon == null)
                    continue;

                if (weapon != this && weapon.cooldownTimer > 0)
                {
                    applyBonus = true;
                    break;
                }
            }

            if (applyBonus)
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
