using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon/On Cooldown")]
public class OnCooldownWeapon : WeaponData
{
    [SerializeField] private int damageBonus;

    public override int CalculateDamage(EntityData holderData, EntityData targetData)
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

        return applyBonus ? damage + damageBonus : damage;
    }
}
