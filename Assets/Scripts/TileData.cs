using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType { Floor, Wall, Entrance, Exit }

[System.Serializable]
public class TileData
{
    public Vector3Int position;
    public TileType type;
    public EntityData entityData;
    public WeaponData weapon;
}
