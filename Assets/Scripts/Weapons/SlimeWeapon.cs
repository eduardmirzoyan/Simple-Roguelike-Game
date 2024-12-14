using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon/Enemy/Slime")]
public class SlimeWeapon : WeaponData
{
    public override int CalculateDamage(EntityData holderData, EntityData targetData)
    {
        float multiplier = (1 + holderData.currentFocus) * 0.5f;
        return (int)(damage * multiplier);
    }

    public override Vector3Int[] CalculateArea(EntityData holderData, Vector3Int position)
    {
        // If not maxed, then just do target tile
        if (holderData.currentFocus < holderData.maxFocus)
            return new Vector3Int[] { position };

        Vector3Int holderPosition = holderData.tileData.position;
        List<Vector3Int> area = new();
        foreach (var direction in Pathfinder.DIRECTIONS)
            area.Add(holderPosition + direction);

        return area.ToArray();
    }
}
