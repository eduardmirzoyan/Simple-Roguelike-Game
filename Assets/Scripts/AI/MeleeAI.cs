using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Melee")]
public class MeleeAI : AI
{
    public override void PerformTurn(EnemyData enemyData, WorldData worldData)
    {
        // Chase player until death do us part
        Vector3Int startPosition = enemyData.tileData.position;
        Vector3Int targetPosition = worldData.player.tileData.position;

        // If enemy has a weapon
        foreach (var weapon in enemyData.weapons)
        {
            // Attempt to attack player
            if (weapon != null)
            {
                // If target in vision
                if (enemyData.vision.visiblePositions.ContainsKey(targetPosition))
                {
                    // If target in range
                    if (Pathfinder.ManhattanDistance(startPosition, targetPosition) <= weapon.range)
                    {
                        // Attack the player
                        GameManager.instance.EntityPerformAttack(enemyData, enemyData.weapons.IndexOf(weapon), targetPosition);
                        return;
                    }
                }
            }
        }

        // path[0] - current position
        // path[1] - next position to move to
        var path = enemyData.pathfinder.FindPath(startPosition, targetPosition, worldData);

        // If we have a valid path
        if (path.Count > 2)
        {
            Vector3Int nextPosition = path[1];
            Vector3Int optimalDirection = nextPosition - startPosition;

            // Move
            GameManager.instance.MoveEntity(enemyData, optimalDirection);
        }
        else
        {
            // Skip turn
            GameManager.instance.SkipTurn(enemyData);
        }
    }
}
