using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldData
{
    public TileData[,] tiles;
    public PlayerData player;
    public List<EnemyData> enemies;
    public List<EnemyData> neutrals;

    public WorldData()
    {
        tiles = new TileData[0, 0];
        enemies = new List<EnemyData>();
        neutrals = new List<EnemyData>();
        player = null;
    }
}
