using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon/Player/Mace")]
public class MaceWeapon : WeaponData
{
    public override Vector3Int[] CalculateArea(EntityData holderData, Vector3Int position)
    {
        // If not maxed, then just do target tile
        if (holderData.currentFocus < holderData.maxFocus)
            return new Vector3Int[] { position };

        Vector3Int holderPosition = holderData.Position;
        List<Vector3Int> area = new();
        foreach (var direction in Pathfinder.DIRECTIONS)
            area.Add(holderPosition + direction);

        return area.ToArray();
    }
}
