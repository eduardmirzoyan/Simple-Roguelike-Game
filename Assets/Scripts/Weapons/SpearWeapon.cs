using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon/Player/Spear")]
public class SpearWeapon : WeaponData
{
    public override Vector3Int[] CalculateArea(EntityData holderData, Vector3Int position)
    {
        // If not maxed, then just do target tile
        if (holderData.currentFocus < holderData.maxFocus)
            return new Vector3Int[] { position };

        // Target two tiles in front
        Vector3Int direction = position - holderData.Position;
        direction.Clamp(-Vector3Int.one, Vector3Int.one);

        Vector3Int forward = position + direction;

        return new Vector3Int[] { position, forward };
    }
}