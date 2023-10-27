using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Chest")]
public class ChestAI : AI
{
    // Does nothing
    public override void PerformTurn(EnemyData enemyData, WorldData worldData)
    {
        GameManager.instance.SkipTurn(enemyData);
    }
}
