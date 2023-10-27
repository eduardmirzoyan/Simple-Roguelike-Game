using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldRenderer : MonoBehaviour
{
    [Header("Generator")]
    [SerializeField] private WorldGenerator worldGenerator;

    [Header("References")]
    [SerializeField] private Tilemap floorTilemap;
    [SerializeField] private Tilemap wallTilemap;
    [SerializeField] private Tilemap entityTilemap;
    [SerializeField] private Tilemap debugTilemap;
    [SerializeField] private Tile floorTile;
    [SerializeField] private RuleTile wallTile;
    [SerializeField] private Tile entranceTile;
    [SerializeField] private Tile exitTile;

    [Header("Settings")]
    [SerializeField] private int padding;

    [Header("Prefabs")]
    [SerializeField] private GameObject entityPrefab;
    [SerializeField] private GameObject weaponPrefab;

    public static WorldRenderer instance;
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

    public void GenerateWorld(WorldData worldData)
    {
        ClearWorld();

        // var worldData = worldGenerator.Generate();

        RenderWorld(worldData);

        MapBoundaryManager.instance.GenerateBoundary();
    }

    public void ClearWorld()
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
        entityTilemap.ClearAllTiles();

        debugTilemap.ClearAllTiles();
    }

    private void DebugRooms(Room[,] rooms)
    {
        // Fill with walls first
        for (int i = 0; i < worldGenerator.gridWidth; i++) // FIX THIS
        {
            for (int j = 0; j < worldGenerator.gridHeight; j++)
            {
                Vector3Int position = new Vector3Int(i, j);
                debugTilemap.SetTile(position, wallTile);
            }
        }

        foreach (var room in rooms)
        {
            if (room == null)
                continue;

            Vector3Int position = room.gridPosition;
            switch (room.roomType)
            {
                case RoomType.Standard: // Standard
                    debugTilemap.SetTile(position, floorTile);
                    debugTilemap.SetTileFlags(position, TileFlags.None);
                    debugTilemap.SetColor(position, Color.white);
                    break;
                case RoomType.Entrance:  // Entrance
                    debugTilemap.SetTile(position, floorTile);
                    debugTilemap.SetTileFlags(position, TileFlags.None);
                    debugTilemap.SetColor(position, Color.cyan);
                    break;
                case RoomType.Exit:  // Exit
                    debugTilemap.SetTile(position, floorTile);
                    debugTilemap.SetTileFlags(position, TileFlags.None);
                    debugTilemap.SetColor(position, Color.red);
                    break;
                case RoomType.Chest:  // Chest Room
                    debugTilemap.SetTile(position, floorTile);
                    debugTilemap.SetTileFlags(position, TileFlags.None);
                    debugTilemap.SetColor(position, Color.yellow);
                    break;
                case RoomType.Branch:  // Branch
                    debugTilemap.SetTile(position, floorTile);
                    debugTilemap.SetTileFlags(position, TileFlags.None);
                    debugTilemap.SetColor(position, Color.gray);
                    break;
            }
        }
    }

    private void RenderWorld(WorldData worldData)
    {
        for (int i = -padding; i < worldData.tiles.GetLength(0) + padding; i++)
        {
            for (int j = -padding; j < worldData.tiles.GetLength(1) + padding; j++)
            {
                Vector3Int position = new Vector3Int(i, j);

                // If out of bounds, then use as padding
                if (WorldGenerator.OutOfBounds(position, worldData.tiles))
                {
                    wallTilemap.SetTile(position, wallTile);
                    continue;
                }

                var tile = worldData.tiles[i, j];
                switch (tile.type)
                {
                    case TileType.Floor:
                        floorTilemap.SetTile(position, floorTile);
                        break;
                    case TileType.Wall:
                        wallTilemap.SetTile(position, wallTile);
                        break;
                    case TileType.Entrance:
                        floorTilemap.SetTile(position, entranceTile);
                        break;
                    case TileType.Exit:
                        floorTilemap.SetTile(position, exitTile);
                        break;
                }

                if (tile.entityData != null)
                {
                    // Spawn entity
                    Vector3 worldPosition = entityTilemap.GetCellCenterWorld(position);
                    Instantiate(entityPrefab, worldPosition, Quaternion.identity, entityTilemap.transform).GetComponent<EntityRenderer>().Initialize(tile.entityData);
                }
            }
        }
    }

    public void SpawnWeapon(WeaponData weapon, Vector3Int position)
    {
        Vector3 worldPosition = entityTilemap.GetCellCenterWorld(position);
        var renderer = Instantiate(weaponPrefab, worldPosition, Quaternion.identity, entityTilemap.transform).GetComponent<WeaponRenderer>();
        renderer.Initialize(weapon);
        weapon.renderer = renderer;
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 postion;
        Vector3 size;

        int gridWidth = worldGenerator.gridWidth;
        int gridHeight = worldGenerator.gridWidth;
        int roomSize = worldGenerator.roomSize;
        int gapSize = worldGenerator.gapSize;

        // Show Grid Cells
        Gizmos.color = Color.red;
        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridHeight; j++)
            {
                Vector3Int cellPosition = new Vector3Int(i, j);
                postion = gapSize * (cellPosition + Vector3Int.one) + roomSize * cellPosition + new Vector3(roomSize / 2f, roomSize / 2f);
                size = new Vector3(roomSize, roomSize);
                Gizmos.DrawWireCube(postion, size);
            }
        }

        // Show minimap
        Gizmos.color = Color.blue;
        postion = new Vector3(gridWidth / 2f, gridHeight / 2f);
        size = new Vector3(gridWidth, gridHeight);
        Gizmos.DrawWireCube(postion, size);
    }
}
