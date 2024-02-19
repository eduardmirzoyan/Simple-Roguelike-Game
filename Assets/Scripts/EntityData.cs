using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityData : ScriptableObject
{
    [Header("Basic Data")]
    public new string name;

    [Header("Health")]
    public int maxHealth;
    public int currentHealth;

    [Header("Posture")]
    public int maxPosture;
    public int currentPosture;
    public bool canRecoverPosture;

    [Header("Weapons")]
    public List<WeaponData> weapons = new List<WeaponData>();
    public int weaponCap = 2;

    [Header("Vision")]
    public Vision vision;

    [Header("Visuals")]
    public Sprite[] idleSprites;
    public Sprite hitSprite;
    public Sprite[] deathSprites;

    [Header("Runtime Data")]
    public EntityRenderer entityRenderer;
    public TileData tileData;
    public WorldData worldData;

    public virtual void Initialize(TileData tileData, WorldData worldData)
    {
        this.tileData = tileData;
        this.worldData = worldData;

        vision.Initialize(this);

        tileData.entityData = this;
    }

    public virtual EntityData Copy()
    {
        var copy = Instantiate(this);

        if (weapons.Count > weaponCap)
            throw new System.Exception("TOO MANY WEAPONS!");

        for (int i = 0; i < weapons.Count; i++)
        {
            if (weapons[i] != null)
                copy.weapons[i] = weapons[i].Copy();
        }

        return copy;
    }

    public override string ToString()
    {
        return $"-{name} ({currentPosture}/{maxPosture}) [{currentHealth}/{maxHealth}]-";
    }
}
