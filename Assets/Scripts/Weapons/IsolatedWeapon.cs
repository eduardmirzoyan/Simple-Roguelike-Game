using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon/Isolated")]
public class IsolatedWeapon : WeaponData
{
    public int bonusDamage;

    public override int CalculateDamage(EntityData holderData, EntityData targetData)
    {
        bool applyBonus = true;

        // Cache values
        var worldTiles = targetData.worldData.tiles;
        var position = targetData.tileData.position;

        // Check cardinal directions
        foreach (var direction in new Vector3Int[] { Vector3Int.left, Vector3Int.up, Vector3Int.right, Vector3Int.down })
        {
            var newPostion = position + direction;

            if (WorldGenerator.OutOfBounds(newPostion, worldTiles))
                continue;

            var tile = worldTiles[newPostion.x, newPostion.y];

            // If invalid
            if (tile.entityData != null || tile.type == TileType.Wall)
            {
                applyBonus = false;
                break;
            }
        }

        return applyBonus ? damage + bonusDamage : damage;
    }
}
