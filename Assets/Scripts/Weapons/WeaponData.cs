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

    public virtual int CalculateDamage(EntityData holderData, EntityData targetData)
    {
        return damage;
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
