using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameEvents : MonoBehaviour
{
    public event Action<EntityData> onTurnStart;
    public event Action<EntityData> onTurnEnd;

    public event Action<TileData> onTileEnter;

    public event Action<int, WeaponData, TileData> onWeaponDrop;
    public event Action<int, WeaponData, TileData> onWeaponPickup;

    public event Action<WeaponData> onAttackSelect;
    public event Action<WeaponData> onAttackCancel;

    public event Action onActionStart;
    public event Action onActionEnd;

    public event Action<EntityData> onEntityResourceChange;

    public event Action<EnemyData> onEntityInspect;

    public static GameEvents instance;
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

    public void TriggerOnTurnStart(EntityData entityData)
    {
        if (onTurnStart != null)
        {
            onTurnStart(entityData);
        }
    }

    public void TriggerOnTurnEnd(EntityData entityData)
    {
        if (onTurnEnd != null)
        {
            onTurnEnd(entityData);
        }
    }

    public void TriggerOnTileEnter(TileData tileData)
    {
        if (onTileEnter != null)
        {
            onTileEnter(tileData);
        }
    }

    public void TriggerOnAttackSelect(WeaponData weaponData)
    {
        if (onAttackSelect != null)
        {
            onAttackSelect(weaponData);
        }
    }

    public void TriggerOnAttackCancel(WeaponData weaponData)
    {
        if (onAttackCancel != null)
        {
            onAttackCancel(weaponData);
        }
    }

    public void TriggerOnActionStart()
    {
        if (onActionStart != null)
        {
            onActionStart();
        }
    }

    public void TriggerOnActionEnd()
    {
        if (onActionEnd != null)
        {
            onActionEnd();
        }
    }

    public void TriggerOnWeaponDrop(int index, WeaponData weaponData, TileData tileData)
    {
        if (onWeaponDrop != null)
        {
            onWeaponDrop(index, weaponData, tileData);
        }
    }

    public void TriggerOnWeaponPickup(int index, WeaponData weaponData, TileData tileData)
    {
        if (onWeaponPickup != null)
        {
            onWeaponPickup(index, weaponData, tileData);
        }
    }

    public void TriggerOnEntityResourceChange(EntityData entityData)
    {
        if (onEntityResourceChange != null)
        {
            onEntityResourceChange(entityData);
        }
    }

    public void TriggerOnEnemyInspect(EnemyData enemyData)
    {
        if (onEntityInspect != null)
        {
            onEntityInspect(enemyData);
        }
    }
}
