using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Intent { Dormant, Alert, Move, Focus, Attack }

public abstract class AI : ScriptableObject
{
    [ReadOnly] public Intent intent;
    [ReadOnly] public Vector3Int attackPosition;

    public abstract void CalculateIntent(EnemyData enemyData, WorldData worldData);

    public AI Copy()
    {
        return Instantiate(this);
    }
}
