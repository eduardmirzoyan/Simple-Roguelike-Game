using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon/Player/Sword")]
public class SwordWeapon : WeaponData
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

        Vector3Int direction = position - holderData.tileData.position;
        direction.Clamp(-Vector3Int.one, Vector3Int.one);
        Vector3Int rotateRight = new(0, 0, 1);
        Vector3Int rotateLeft = new(0, 0, -1);
        Vector3Int leftLocation = position + Vector3Int.RoundToInt(Vector3.Cross(direction, rotateLeft));
        Vector3Int rightLocation = position + Vector3Int.RoundToInt(Vector3.Cross(direction, rotateRight));

        return new Vector3Int[] { leftLocation, position, rightLocation };
    }
}
