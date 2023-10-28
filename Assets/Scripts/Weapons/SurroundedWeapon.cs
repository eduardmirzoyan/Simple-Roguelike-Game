using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon/Surrounded")]
public class SurroundedWeapon : WeaponData
{
    public override int CalculateDamage(EntityData holderData, EntityData targetData)
    {
        int bonus = 0;

        // Check each neighbor
        var worldTiles = targetData.worldData.tiles;
        var position = targetData.tileData.position;

        // Check cardinal directions
        foreach (var direction in new Vector3Int[] { Vector3Int.left, Vector3Int.up, Vector3Int.right, Vector3Int.down })
        {
            var newPostion = position + direction;

            if (WorldGenerator.OutOfBounds(newPostion, worldTiles))
                continue;

            var tile = worldTiles[newPostion.x, newPostion.y];
            if (tile.entityData != null)
                bonus++;
        }

        return damage + bonus;
    }
}
