using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Slime")]
public class SlimeAI : AI
{
    private enum SlimeState { Dormant, Awake }
    [SerializeField] private SlimeState slimeState;

    public override void PerformTurn(EnemyData enemyData, WorldData worldData)
    {
        switch (slimeState)
        {
            case SlimeState.Dormant:

                // If this enemy is in player vision
                if (worldData.player.vision.visiblePositions.ContainsKey(enemyData.tileData.position))
                {
                    enemyData.entityRenderer.Aggro(true);

                    // Set awake
                    slimeState = SlimeState.Awake;
                }

                // Skip turn
                GameManager.instance.SkipTurn(enemyData);

                break;
            case SlimeState.Awake:

                enemyData.entityRenderer.Aggro(false);

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

                break;
        }

    }
}
