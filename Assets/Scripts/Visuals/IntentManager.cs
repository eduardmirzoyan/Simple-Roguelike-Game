using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class IntentManager : MonoBehaviour
{
    [SerializeField] private Tilemap intentTilemap;
    [SerializeField] private AnimatedTile intentTile;

    Dictionary<Vector3Int, int> intentTable;

    public static IntentManager instance;
    private void Awake()
    {
        // Singleton logic
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;

        intentTable = new();
        intentTilemap.ClearAllTiles();
    }

    public void Target(Vector3Int[] positions)
    {
        foreach (var position in positions)
        {
            if (intentTable.ContainsKey(position))
            {
                intentTable[position]++;
            }
            else
            {
                intentTable[position] = 1;
                intentTilemap.SetTile(position, intentTile);
            }
        }
    }

    public void Clear(Vector3Int[] positions)
    {
        foreach (var position in positions)
        {
            if (intentTable.ContainsKey(position))
            {
                intentTable[position]--;
                if (intentTable[position] == 0)
                {
                    intentTable.Remove(position);
                    intentTilemap.SetTile(position, null);
                }
            }
            else throw new System.Exception("Could clear intent at position: " + position);
        }
    }
}
