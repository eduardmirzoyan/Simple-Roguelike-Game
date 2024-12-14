using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Basic")]
public class BasicAI : AI
{
    [SerializeField] private float extraFocusChance = 0.33f;

    public override void CalculateIntent(EnemyData enemyData, WorldData worldData)
    {
        var playerData = worldData.player;
        Vector3Int enemyPosition = enemyData.tileData.position;
        Vector3Int targetPosition = playerData.tileData.position;
        var weapon = enemyData.Weapon;

        switch (intent)
        {
            case Intent.Dormant:

                // If this enemy is in player's vision, then wake up
                if (playerData.vision.visiblePositions.ContainsKey(enemyPosition))
                    intent = Intent.Alert;

                break;
            default:

                // Check if player is out of range
                int distance = Pathfinder.ManhattanDistance(enemyPosition, targetPosition);
                if (distance > weapon.range)
                {
                    intent = Intent.Move;
                }
                // Check if you have at least 1 focus and target in range
                else if (enemyData.currentFocus > 0)
                {
                    // If not at max focus, then small chance to focus again
                    if (enemyData.currentFocus < enemyData.maxFocus && Random.Range(0f, 1f) <= extraFocusChance)
                    {
                        intent = Intent.Focus;
                        return;
                    }

                    intent = Intent.Attack;
                    attackPosition = targetPosition;
                }
                else
                {
                    intent = Intent.Focus;
                }
                break;
        }
    }
}
