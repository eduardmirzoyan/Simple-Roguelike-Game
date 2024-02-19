using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameEvents : MonoBehaviour
{
    public event Action<EntityData> onTurnStart;
    public event Action<EntityData> onTurnEnd;

    public event Action<EntityData, TileData> onTileEnter;

    public event Action<int, WeaponData, TileData> onWeaponDrop;
    public event Action<int, WeaponData, TileData> onWeaponPickup;

    public event Action<WeaponData> onAttackSelect;
    public event Action<WeaponData> onAttackCancel;

    public event Action onActionStart;
    public event Action onActionEnd;

    public event Action<EntityData, WeaponData, TileData> onEntityAttackTile;

    public event Action<EntityData> onEntityResourceChange;

    public event Action<EnemyData> onEntityInspect;

    public event Action<EnemyData> onEnemyChangeState;
    public event Action<int> onGameOver;

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
        onTurnStart?.Invoke(entityData);
    }

    public void TriggerOnTurnEnd(EntityData entityData)
    {
        onTurnEnd?.Invoke(entityData);
    }

    public void TriggerOnTileEnter(EntityData entityData, TileData tileData)
    {
        onTileEnter?.Invoke(entityData, tileData);
    }

    public void TriggerOnAttackSelect(WeaponData weaponData)
    {
        onAttackSelect?.Invoke(weaponData);
    }

    public void TriggerOnAttackCancel(WeaponData weaponData)
    {
        onAttackCancel?.Invoke(weaponData);
    }

    public void TriggerOnActionStart()
    {
        onActionStart?.Invoke();
    }

    public void TriggerOnActionEnd()
    {
        onActionEnd?.Invoke();
    }

    public void TriggerOnWeaponDrop(int index, WeaponData weaponData, TileData tileData)
    {
        onWeaponDrop?.Invoke(index, weaponData, tileData);
    }

    public void TriggerOnWeaponPickup(int index, WeaponData weaponData, TileData tileData)
    {
        onWeaponPickup?.Invoke(index, weaponData, tileData);
    }

    public void TriggerOnEntityAttackTile(EntityData entityData, WeaponData weaponData, TileData tileData)
    {
        onEntityAttackTile?.Invoke(entityData, weaponData, tileData);
    }

    public void TriggerOnEntityResourceChange(EntityData entityData)
    {
        onEntityResourceChange?.Invoke(entityData);
    }

    public void TriggerOnEnemyInspect(EnemyData enemyData)
    {
        onEntityInspect?.Invoke(enemyData);
    }

    public void TriggerOnEnemyChangeState(EnemyData enemyData)
    {
        onEnemyChangeState?.Invoke(enemyData);
    }

    public void TriggerOnGameOver(int score)
    {
        onGameOver?.Invoke(score);
    }
}
