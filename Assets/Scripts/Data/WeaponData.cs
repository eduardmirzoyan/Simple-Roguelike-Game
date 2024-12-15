using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponData : ScriptableObject
{
    [Header("Basic Data")]
    public new string name;
    [TextArea(5, 1)] public string description;
    public Sprite sprite;

    [Header("Weapon Data")]
    public Vector2Int damageRange;
    public int range;

    [Header("Runtime Data")]
    [ReadOnly] public WeaponRenderer renderer;

    public Vector2Int GetTrueDamageRange(EntityData holderData)
    {
        int damageMultiplier = 1;
        if (holderData.currentFocus > 0)
            damageMultiplier = 2;

        return damageMultiplier * damageRange;
    }

    public virtual int CalculateDamage(EntityData holderData)
    {
        var range = GetTrueDamageRange(holderData);

        return Random.Range(range.x, range.y);
    }

    public virtual Vector3Int[] CalculateArea(EntityData holderData, Vector3Int position)
    {
        return new Vector3Int[] { position };
    }

    public virtual void Initialize(EntityData holder)
    {
        // Used to store data for other weapons
    }

    public virtual void Use() { }

    public virtual void Unintialize() { }

    public WeaponData Copy()
    {
        return Instantiate(this);
    }
}
