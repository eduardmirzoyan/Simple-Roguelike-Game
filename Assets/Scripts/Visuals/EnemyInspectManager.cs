using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyInspectManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Tilemap inspectTilemap;
    [SerializeField] private RuleTile highlightTile;

    [Header("Settings")]
    [SerializeField] private Color highlightColor;

    [Header("Debug")]
    [SerializeField] private EnemyData enemyData;

    public static EnemyInspectManager instance;
    private void Awake()
    {
        // Singleton logic
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
    }

    public void InspectTile(TileData tileData)
    {
        if (tileData == null)
            throw new System.Exception("Input was null!");

        // If we have a previous enemy, clear it (or input null)
        if (this.enemyData != null)
        {
            GameEvents.instance.TriggerOnEnemyInspect(null);

            inspectTilemap.ClearAllTiles();

            this.enemyData = null;
        }

        // If we have a new enemy
        if (tileData.entityData is EnemyData)
        {
            var enemyData = tileData.entityData as EnemyData;
            var tiles = enemyData.worldData.tiles;

            GameEvents.instance.TriggerOnEnemyInspect(enemyData);

            foreach (var position in enemyData.vision.visiblePositions.Keys)
            {
                if (tiles[position.x, position.y].type != TileType.Wall)
                {
                    inspectTilemap.SetTile(position, highlightTile);
                    inspectTilemap.SetTileFlags(position, TileFlags.None);
                    inspectTilemap.SetColor(position, highlightColor);
                }
            }

            this.enemyData = enemyData;
        }
    }

    public void ResetInspect()
    {
        GameEvents.instance.TriggerOnEnemyInspect(null);

        inspectTilemap.ClearAllTiles();

        this.enemyData = null;
    }
}
