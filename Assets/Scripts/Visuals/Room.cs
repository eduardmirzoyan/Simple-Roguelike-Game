using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType { Standard, Entrance, Exit, Branch, Chest }

public class Room
{
    public Vector3Int gridPosition;
    public int[,] tileGrid;
    public int roomSize;
    public List<Room> neighbors;

    public RoomType roomType;

    public Room(Vector3Int gridPosition, int roomSize)
    {
        this.gridPosition = gridPosition;
        this.roomSize = roomSize;
        neighbors = new List<Room>();
    }

    public void DefineType(RoomType roomType)
    {
        this.roomType = roomType;

        // 0 = Floor | 1 = Wall | 2 = Entrance | 3 = Exit | 4 = Chest | 5 = Enemy | 6 = Vase
        tileGrid = new int[roomSize, roomSize];

        int wallChance = 10;
        int count;
        switch (roomType)
        {
            case RoomType.Standard:

                // Randomly place walls in center
                for (int i = 1; i < roomSize - 1; i++)
                {
                    for (int j = 1; j < roomSize - 1; j++)
                    {
                        if (Random.Range(0, 100) <= wallChance)
                            tileGrid[i, j] = 1;
                        else
                            tileGrid[i, j] = 0;
                    }
                }

                // Randomly place enemies
                count = Random.Range(1, 3); // 1 or 2
                do
                {
                    // Select random position in grid
                    Vector2Int position = new Vector2Int(Random.Range(0, roomSize), Random.Range(0, roomSize));

                    // Position is open
                    if (tileGrid[position.x, position.y] == 0)
                    {
                        tileGrid[position.x, position.y] = 5;
                        count--;
                    }

                } while (count > 0);

                // Randomly place vases
                count = Random.Range(1, 3); // 1 or 2
                do
                {
                    // Select random position in grid
                    Vector2Int position = new Vector2Int(Random.Range(0, roomSize), Random.Range(0, roomSize));

                    // Position is open
                    if (tileGrid[position.x, position.y] == 0)
                    {
                        tileGrid[position.x, position.y] = 6;
                        count--;
                    }

                } while (count > 0);

                break;
            case RoomType.Entrance:

                // Place entrance in center
                tileGrid[roomSize / 2, roomSize / 2] = 2;

                // Randomly place vases
                count = Random.Range(1, 3); // 1 or 2
                do
                {
                    // Select random position in grid
                    Vector2Int position = new Vector2Int(Random.Range(0, roomSize), Random.Range(0, roomSize));

                    // Position is open
                    if (tileGrid[position.x, position.y] == 0)
                    {
                        tileGrid[position.x, position.y] = 6;
                        count--;
                    }

                } while (count > 0);

                break;
            case RoomType.Exit:

                // Place exit in center
                tileGrid[roomSize / 2, roomSize / 2] = 3;

                // Randomly place vases
                count = Random.Range(1, 3); // 1 or 2
                do
                {
                    // Select random position in grid
                    Vector2Int position = new Vector2Int(Random.Range(0, roomSize), Random.Range(0, roomSize));

                    // Position is open
                    if (tileGrid[position.x, position.y] == 0)
                    {
                        tileGrid[position.x, position.y] = 6;
                        count--;
                    }

                } while (count > 0);

                break;
            case RoomType.Chest:

                // Place chest in center
                tileGrid[roomSize / 2, roomSize / 2] = 4;

                break;
            case RoomType.Branch:

                // Randomly place walls in center
                for (int i = 1; i < roomSize - 1; i++)
                {
                    for (int j = 1; j < roomSize - 1; j++)
                    {
                        if (Random.Range(0, 100) <= wallChance)
                            tileGrid[i, j] = 1;
                        else
                            tileGrid[i, j] = 0;
                    }
                }

                // Randomly place enemies
                count = Random.Range(1, 3); // 1 or 2
                do
                {
                    // Select random position in grid
                    Vector2Int position = new Vector2Int(Random.Range(0, roomSize), Random.Range(0, roomSize));

                    // Position is open
                    if (tileGrid[position.x, position.y] == 0)
                    {
                        tileGrid[position.x, position.y] = 5;
                        count--;
                    }

                } while (count > 0);

                // Randomly place vases
                count = Random.Range(1, 3); // 1 or 2
                do
                {
                    // Select random position in grid
                    Vector2Int position = new Vector2Int(Random.Range(0, roomSize), Random.Range(0, roomSize));

                    // Position is open
                    if (tileGrid[position.x, position.y] == 0)
                    {
                        tileGrid[position.x, position.y] = 6;
                        count--;
                    }

                } while (count > 0);

                break;

        }
    }

    public void AddNeighbor(Room room)
    {
        neighbors.Add(room);
    }
}