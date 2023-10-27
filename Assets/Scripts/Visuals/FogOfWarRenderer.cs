using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FogOfWarRenderer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Tilemap hiddenTilemap;
    [SerializeField] private Tilemap exploredTilemap;
    [SerializeField] private Tile whiteTile;
    [SerializeField] private RuleTile fogRuleTile;
    [SerializeField] private Color hiddenColor;
    [SerializeField] private Color exploredColor;

    public static FogOfWarRenderer instance;
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

    public void Initialize(int worldSize)
    {
        int padding = 5;
        for (int i = -padding; i < worldSize + padding; i++)
        {
            for (int j = -padding; j < worldSize + padding; j++)
            {
                Vector3Int position = new Vector3Int(i, j);
                hiddenTilemap.SetTile(position, fogRuleTile);
                hiddenTilemap.SetTileFlags(position, TileFlags.None);
                hiddenTilemap.SetColor(position, hiddenColor);

                exploredTilemap.SetTile(position, fogRuleTile);
                exploredTilemap.SetTileFlags(position, TileFlags.None);
                exploredTilemap.SetColor(position, exploredColor);
            }
        }
    }

    public void UpdateFog(Vision vision)
    {
        // Set visible positions
        foreach (var position in vision.visiblePositions.Keys)
        {
            hiddenTilemap.SetTile(position, null);
            exploredTilemap.SetTile(position, null);
        }

        // Set explored positions
        foreach (var previousPosition in vision.previousPositions.Keys)
        {
            // If existed previously, but not now
            if (!vision.visiblePositions.ContainsKey(previousPosition))
            {
                // Set to explored
                exploredTilemap.SetTile(previousPosition, fogRuleTile);
                exploredTilemap.SetTileFlags(previousPosition, TileFlags.None);
                exploredTilemap.SetColor(previousPosition, exploredColor);
            }
        }
    }
}
