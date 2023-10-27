using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Vision
{
    // Stores data about what can be seen
    [Header("Settings")]
    [SerializeField] private int viewDistance;

    [Header("Debug")]
    [SerializeField, ReadOnly] private EntityData entityData;
    public Dictionary<Vector3Int, bool> previousPositions;
    public Dictionary<Vector3Int, bool> visiblePositions;

    private FieldOfView fov;

    public void Initialize(EntityData entityData)
    {
        this.entityData = entityData;

        fov = new FieldOfView(BlocksLight, SetVisible, GetDistance);

        previousPositions = new Dictionary<Vector3Int, bool>();
        visiblePositions = new Dictionary<Vector3Int, bool>();
    }

    public void Refresh()
    {
        // Update previous
        previousPositions = visiblePositions;

        // Clear current
        visiblePositions = new Dictionary<Vector3Int, bool>();

        // Find visible tiles
        fov.Compute(entityData.tileData.position, viewDistance);
    }

    private int GetDistance(int x, int y)
    {
        return x + y;
    }

    private void SetVisible(int x, int y)
    {
        Vector3Int position = new Vector3Int(x, y);

        // Add to table
        visiblePositions[position] = true;
    }

    private bool BlocksLight(int x, int y)
    {
        var world = entityData.worldData.tiles;

        // If wall exists, then block light
        return world[x, y].type == TileType.Wall;
    }
}
