using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon/Max Range")]
public class MaxRangeWeapon : WeaponData
{
    [SerializeField] private int damageBonus;

    public override int GetTotalDamage(EntityData holderData, EntityData targetData)
    {
        int distance = Pathfinder.ManhattanDistance(holderData.tileData.position, targetData.tileData.position);

        return distance == range ? damage + damageBonus : damage;
    }
}
