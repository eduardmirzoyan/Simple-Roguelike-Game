using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Pathfinder
{
    public static readonly Vector3Int[] DIRECTIONS = new Vector3Int[] { Vector3Int.left, Vector3Int.up, Vector3Int.right, Vector3Int.down };

    private class Node
    {
        public Vector3Int location;

        public int G;
        public int H;
        public int F { get { return G + H; } }
        public Node previous;

        public Node(Vector3Int location)
        {
            this.location = location;
            previous = null;
        }
    }

    public static List<Vector3Int> FindPath(Vector3Int start, Vector3Int end, WorldData worldData)
    {
        // Find path using A*
        List<Node> open = new(); // Makes sure no copies are kept, idealy should be prio queue
        List<Node> closed = new();

        var startNode = new Node(start);

        // Start from start
        open.Add(startNode);

        while (open.Count > 0)
        {
            // Sort by F value then get first item
            var currentNode = open.OrderBy(node => node.F).First();

            open.Remove(currentNode);
            closed.Add(currentNode);

            if (currentNode.location == end)
            {
                // Debug
                // Debug.Log(open.Count);
                // string debug = "Open: ";
                // foreach (var location in open)
                // {
                //     debug += location.location + " ";
                // }
                // Debug.Log(debug);

                // Return finalized path
                return GetFinalPath(startNode, currentNode);
            }

            // Get all valid neightbors
            var neighbors = GetValidNeightbors(currentNode.location, end, worldData);
            foreach (var neighbor in neighbors)
            {
                // Make node
                var neighborNode = new Node(neighbor);

                // Skip if closed contains node
                if (closed.Any(node => node.location == neighbor))
                {
                    continue;
                }

                // Update values
                neighborNode.G = ManhattanDistance(start, neighbor); // G
                neighborNode.H = ManhattanDistance(end, neighbor); // H

                // Update previous
                neighborNode.previous = currentNode;

                // Make sure no copies exist
                if (!open.Any(node => node.location == neighbor))
                    open.Add(neighborNode);
            }
        }

        Debug.Log("No valid path found.");
        // Return empty list
        return new List<Vector3Int>();
    }

    private static List<Vector3Int> GetValidNeightbors(Vector3Int location, Vector3Int end, WorldData worldData)
    {
        List<Vector3Int> neighbors = new();

        foreach (var direction in DIRECTIONS)
        {
            Vector3Int newLocation = location + direction;

            // Make sure in bounds
            if (WorldGenerator.OutOfBounds(newLocation, worldData.tiles))
                continue;

            // Make sure no wall
            if (worldData.tiles[newLocation.x, newLocation.y].type == TileType.Wall)
                continue;

            // Make sure no entity
            if (newLocation != end && worldData.tiles[newLocation.x, newLocation.y].entityData != null)
                continue;

            neighbors.Add(newLocation);
        }

        return neighbors;
    }

    private static List<Vector3Int> GetFinalPath(Node start, Node end)
    {
        List<Vector3Int> result = new();

        var current = end;

        while (current != null)
        {
            // Add location
            result.Add(current.location);
            // Increment
            current = current.previous;
        }

        // Reverse list
        result.Reverse();

        return result;
    }

    public static int ManhattanDistance(Vector3Int point1, Vector3Int point2)
    {
        return Mathf.Abs(point1.x - point2.x) + Mathf.Abs(point1.y - point2.y);
    }
}
