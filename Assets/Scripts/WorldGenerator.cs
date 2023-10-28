using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class WorldGenerator : ScriptableObject
{
    [Header("Generation Details")]
    public int gridWidth;
    public int gridHeight;
    public int roomSize;
    public int numRooms;
    [Range(0, 100)] public int branchChance;
    public int gapSize;
    public int hallwaySize;

    [Header("Generators")]
    public EnemyGenerator enemyGenerator;

    [Header("Entities")]
    [SerializeField] private EntityData playerData;

    public static Vector2Int[] DIRECTIONS = new Vector2Int[] { Vector2Int.left, Vector2Int.up, Vector2Int.right, Vector2Int.down };

    public int[,] GenerateGrid(int gridWidth, int gridHeight, int numRooms, int branchChance)
    {
        // 0 == Empty | 1 == Standard Room | 2 = Start Room | 3 = End Room | 4 = Branch Room
        int[,] grid;

        // Repeat process until valid generation is found
        int roomCount;
        do
        {
            grid = new int[gridWidth, gridHeight];

            // Start location is center of grid
            Vector2Int nextPosition = new Vector2Int(gridWidth / 2, gridHeight / 2);

            roomCount = 0;
            while (roomCount < numRooms)
            {
                // If first room
                if (roomCount == 0)
                {
                    // Set current room
                    grid[nextPosition.x, nextPosition.y] = 2;

                    // Find next room
                    // Get all valid neighbors
                    var neighbors = GetValidNeighbors(nextPosition, grid);
                    if (neighbors.Count == 0) // If no valid neighbors then dip
                        break;

                    // Randomly choose one neighbor
                    nextPosition = neighbors[Random.Range(0, neighbors.Count)];
                }
                // If last room
                else if (roomCount == numRooms - 1)
                {
                    // Set current room
                    grid[nextPosition.x, nextPosition.y] = 3;

                    // No next room
                }
                else
                {
                    // Set current room
                    if (roomCount == numRooms / 2 - 1) // If middle-point room
                    {
                        grid[nextPosition.x, nextPosition.y] = 4;
                    }
                    else
                    {
                        grid[nextPosition.x, nextPosition.y] = 1;
                    }

                    // Find next room

                    // Get all valid neighbors
                    var neighbors = GetValidNeighbors(nextPosition, grid);
                    if (neighbors.Count == 0) // If no valid neighbors then dip
                        break;

                    // Randomly choose one neighbor
                    var neighbor = neighbors[Random.Range(0, neighbors.Count)];

                    // Check if we can branch here
                    neighbors.Remove(neighbor);
                    if (neighbors.Count > 0)
                    {
                        // Randomly decide to branch
                        if (Random.Range(0, 100) <= branchChance)
                        {
                            // Branch new room
                            var branch = neighbors[Random.Range(0, neighbors.Count)];
                            grid[branch.x, branch.y] = 5;
                        }
                    }

                    // Update current position
                    nextPosition = neighbor;
                }

                // Increment count
                roomCount++;
            }

        } while (roomCount < numRooms);

        return grid;
    }

    public Room[,] GenerateRooms(int[,] grid, int roomSize)
    {
        Room[,] rooms = new Room[grid.GetLength(0), grid.GetLength(1)];

        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                Vector3Int position = new Vector3Int(i, j);
                Room room = new Room(position, roomSize);

                // 0 == Empty | 1 == Standard Room | 2 = Start Room | 3 = End Room | 4 = Chest Room | 5 = Branch
                switch (grid[i, j])
                {
                    case 0:
                        room = null;
                        break;
                    case 1:
                        room.DefineType(RoomType.Standard);
                        break;
                    case 2:
                        room.DefineType(RoomType.Entrance);
                        break;
                    case 3:
                        room.DefineType(RoomType.Exit);
                        break;
                    case 4:
                        room.DefineType(RoomType.Chest);
                        break;
                    case 5:
                        room.DefineType(RoomType.Branch);
                        break;
                    default:
                        throw new System.Exception("UNHANDLED VALUE");
                }

                rooms[i, j] = room;
            }
        }

        GenerateNeighbors(rooms);

        return rooms;
    }


    private void GenerateNeighbors(Room[,] rooms)
    {
        foreach (var room in rooms)
        {
            if (room == null)
                continue;

            List<Room> neighbors = new List<Room>();

            foreach (var direction in DIRECTIONS)
            {
                var newPositon = (Vector2Int)room.gridPosition + direction;

                // Check out of bounds
                if (OutOfBounds(newPositon, rooms))
                    continue;

                // If room exits nearby
                var neighbor = rooms[newPositon.x, newPositon.y];
                if (neighbor != null)
                {
                    room.AddNeighbor(neighbor);
                }
            }
        }
    }

    public WorldData GenerateWorld(Room[,] rooms, int gapSize, int roomSize, int gridSize, PlayerData existingPlayer)
    {
        WorldData worldData = new WorldData();

        // int gridSize = rooms.GetLength(0);
        // roomSIze= 5
        int worldSize = gridSize * (roomSize + gapSize) + 1;

        TileData[,] tiles = new TileData[worldSize, worldSize];

        for (int i = 0; i < worldSize; i++)
        {
            for (int j = 0; j < worldSize; j++)
            {
                var tile = new TileData()
                {
                    position = new Vector3Int(i, j),
                    type = TileType.Wall,
                };
                tiles[i, j] = tile;
            }
        }

        Dictionary<Room, bool> roomTable = new Dictionary<Room, bool>();
        foreach (var room in rooms)
        {
            if (room == null)
                continue;

            for (int i = 0; i < roomSize; i++)
            {
                for (int j = 0; j < roomSize; j++)
                {
                    Vector3Int worldPosition = gapSize * (room.gridPosition + Vector3Int.one) + room.gridPosition * roomSize + new Vector3Int(i, j);

                    // 0 = Floor | 1 = Wall | 2 = Entrance | 3 = Exit | 4 = Chest | 5 = Enemy | 6 = Vase
                    var tile = tiles[worldPosition.x, worldPosition.y];
                    switch (room.tileGrid[i, j])
                    {
                        case 0:

                            tile.type = TileType.Floor;

                            break;
                        case 1:

                            tile.type = TileType.Wall;

                            break;
                        case 2:

                            tile.type = TileType.Entrance;

                            // Add player here
                            if (existingPlayer == null)
                            {
                                var player = playerData.Copy() as PlayerData;
                                worldData.player = player;
                                player.Initialize(tile, worldData);
                            }
                            else
                            {
                                worldData.player = existingPlayer;
                                existingPlayer.Initialize(tile, worldData);
                            }

                            break;
                        case 3:

                            tile.type = TileType.Exit;

                            break;
                        case 4:

                            tile.type = TileType.Floor;

                            // Add chest here
                            var chest = enemyGenerator.GenerateChest();
                            worldData.neutrals.Add(chest);
                            chest.Initialize(tile, worldData);

                            break;
                        case 5:

                            tile.type = TileType.Floor;

                            // Add enemy here
                            var enemy = enemyGenerator.GenerateEnemy();
                            worldData.enemies.Add(enemy);
                            enemy.Initialize(tile, worldData);

                            break;
                        case 6:
                            tile.type = TileType.Floor;

                            // Add vase here
                            var vase = enemyGenerator.GenerateVase();
                            worldData.neutrals.Add(vase);
                            vase.Initialize(tile, worldData);

                            break;
                    }
                }
            }

            // Now generate hallways

            // Draw a hallway to any adjacent rooms
            foreach (var neighbor in room.neighbors)
            {
                // Skip if we already have room
                if (roomTable.ContainsKey(neighbor))
                    continue;

                // Create hallway between rooms
                Vector3Int roomCenter1 = gapSize * (room.gridPosition + Vector3Int.one) + roomSize * room.gridPosition + new Vector3Int(roomSize / 2, roomSize / 2);
                Vector3Int roomCenter2 = gapSize * (neighbor.gridPosition + Vector3Int.one) + roomSize * neighbor.gridPosition + new Vector3Int(roomSize / 2, roomSize / 2);

                // Set center tile to floor
                Vector3Int centerPosition = (roomCenter1 + roomCenter2) / 2;

                // Calculate next direction
                Vector3 direction = roomCenter2 - roomCenter1;
                direction.Normalize();
                Vector3 rotate = new Vector3(0, 0, -1);
                Vector3Int extendDirection = Vector3Int.RoundToInt(Vector3.Cross(direction, rotate));

                // Find corner of hallway
                Vector3Int cornerPosition = centerPosition - extendDirection * roomSize / 2;

                // Generate random start index
                int index = Random.Range(0, roomSize - hallwaySize + 1);

                // Offset corner
                Vector3Int hallwayPosition = cornerPosition + index * extendDirection;

                for (int i = 0; i < hallwaySize; i++)
                {
                    Vector3Int position = hallwayPosition + extendDirection * i;
                    tiles[position.x, position.y].type = TileType.Floor;
                }
            }

            // Label this room as finished
            roomTable[room] = true;
        }

        worldData.tiles = tiles;

        return worldData;
    }

    public WorldData Generate(PlayerData playerData = null)
    {
        var grid = GenerateGrid(gridWidth, gridHeight, numRooms, branchChance);
        var rooms = GenerateRooms(grid, roomSize);
        return GenerateWorld(rooms, gapSize, roomSize, gridHeight, playerData);
    }


    private List<Vector2Int> GetValidNeighbors(Vector2Int position, int[,] grid)
    {
        List<Vector2Int> validNeighbors = new List<Vector2Int>();

        foreach (var direction in DIRECTIONS)
        {
            var newLocation = position + direction;

            // Skip if out of bounds or wall
            if (OutOfBounds(newLocation, grid) || grid[newLocation.x, newLocation.y] != 0)
                continue;

            bool validNeighbor = true;
            foreach (var neighborDirection in DIRECTIONS)
            {
                var newNewLocation = newLocation + neighborDirection;

                // Skip if out of bounds
                if (OutOfBounds(newNewLocation, grid))
                {
                    continue;
                }

                // If neighbor has another neighbor that isn't original
                if (grid[newNewLocation.x, newNewLocation.y] != 0 && newNewLocation != position)
                {
                    // Then invalid location
                    validNeighbor = false;
                }
            }

            // Add to valid neighbors
            if (validNeighbor)
            {
                validNeighbors.Add(newLocation);
            }
        }

        return validNeighbors;
    }

    public static bool OutOfBounds<T>(Vector2Int position, T[,] grid)
    {
        return position.x < 0 || position.y < 0 || position.x >= grid.GetLength(0) || position.y >= grid.GetLength(1);
    }

    public static bool OutOfBounds<T>(Vector3Int position, T[,] grid)
    {
        return position.x < 0 || position.y < 0 || position.x >= grid.GetLength(0) || position.y >= grid.GetLength(1);
    }
}
