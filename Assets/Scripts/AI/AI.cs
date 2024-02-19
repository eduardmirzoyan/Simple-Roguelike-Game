using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AI : ScriptableObject
{
    public enum State { Dormant, Awake, Aggro }
    public State state;

    public abstract void PerformTurn(EnemyData enemyData, WorldData worldData);

    public AI Copy()
    {
        return Instantiate(this);
    }
}
