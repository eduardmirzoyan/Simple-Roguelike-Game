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
    public int damage;
    public int range;
    public int cooldown;

    [Header("Runtime Data")]
    public WeaponRenderer renderer;
    public int cooldownTimer;
    public int bonusDamage;
    public int totalDamge
    {
        get { return damage + bonusDamage; }
    }

    public void Initialize()
    {
        // TODO
    }

    public void Unintialize()
    {
        // TODO
    }

    public abstract void Attack(EntityData holderData, TileData targetData);

    public WeaponData Copy()
    {
        return Instantiate(this);
    }
}
