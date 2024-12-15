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
            ResetInspect();
        }

        // If we have a new enemy
        if (tileData.entityData is EnemyData)
        {
            var enemyData = tileData.entityData as EnemyData;
            var enemyPosition = enemyData.Position;

            // Highlight tile enemy is on
            inspectTilemap.SetTile(enemyPosition, highlightTile);
            inspectTilemap.SetTileFlags(enemyPosition, TileFlags.None);
            inspectTilemap.SetColor(enemyPosition, highlightColor);

            // Highlight any threatened positions
            if (enemyData.ai != null)
            {
                foreach (var position in enemyData.ai.threatenedPositions)
                {
                    inspectTilemap.SetTile(position, highlightTile);
                    inspectTilemap.SetTileFlags(position, TileFlags.None);
                    inspectTilemap.SetColor(position, highlightColor);
                }
            }

            this.enemyData = enemyData;

            // Trigger related event
            GameEvents.instance.TriggerOnEnemyInspect(enemyData);
        }
    }

    public void ResetInspect()
    {
        GameEvents.instance.TriggerOnEnemyInspect(null);

        inspectTilemap.ClearAllTiles();

        this.enemyData = null;
    }
}
