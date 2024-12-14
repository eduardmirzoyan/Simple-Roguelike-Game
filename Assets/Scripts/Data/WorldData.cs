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
    public int Size
    {
        get { return tiles.GetLength(0); }
    }

    public WorldData()
    {
        tiles = new TileData[0, 0];
        enemies = new List<EnemyData>();
        neutrals = new List<EnemyData>();
        player = null;
    }

    public TileData GetTile(Vector3Int position)
    {
        return tiles[position.x, position.y];
    }

    public bool IsOutOfBounds(Vector3Int position)
    {
        return position.x < 0 || position.y < 0 || position.x >= tiles.GetLength(0) || position.y >= tiles.GetLength(1);
    }
}
