using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameEvents : MonoBehaviour
{
    public event Action<WorldData> OnGenerateWorld;
    public event Action<PlayerData> OnPlayerEnter;

    public event Action<EntityData> OnTurnStart;
    public event Action<EntityData> OnTurnEnd;

    public event Action<EntityData, TileData> OnTileEnter;

    public event Action<int, WeaponData, TileData> OnWeaponDrop;
    public event Action<int, WeaponData, TileData> OnWeaponPickup;

    public event Action<WeaponData> OnAttackSelect;
    public event Action<WeaponData> OnAttackCancel;

    public event Action OnActionStart;
    public event Action OnActionEnd;

    public event Action<EntityData, WeaponData, TileData> OnEntityAttackTile;

    public event Action<EntityData> OnEntityResourceChange;

    public event Action<EnemyData> OnEntityInspect;

    public event Action<EnemyData> OnEnemyIntent;
    public event Action<int> OnGameOver;

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

    public void TriggerOnEnterWorld(WorldData worldData)
    {
        OnGenerateWorld?.Invoke(worldData);
    }

    public void TriggerOnPlayerEnter(PlayerData playerData)
    {
        OnPlayerEnter?.Invoke(playerData);
    }

    public void TriggerOnTurnStart(EntityData entityData)
    {
        OnTurnStart?.Invoke(entityData);
    }

    public void TriggerOnTurnEnd(EntityData entityData)
    {
        OnTurnEnd?.Invoke(entityData);
    }

    public void TriggerOnTileEnter(EntityData entityData, TileData tileData)
    {
        OnTileEnter?.Invoke(entityData, tileData);
    }

    public void TriggerOnAttackSelect(WeaponData weaponData)
    {
        OnAttackSelect?.Invoke(weaponData);
    }

    public void TriggerOnAttackCancel(WeaponData weaponData)
    {
        OnAttackCancel?.Invoke(weaponData);
    }

    public void TriggerOnActionStart()
    {
        OnActionStart?.Invoke();
    }

    public void TriggerOnActionEnd()
    {
        OnActionEnd?.Invoke();
    }

    public void TriggerOnWeaponDrop(int index, WeaponData weaponData, TileData tileData)
    {
        OnWeaponDrop?.Invoke(index, weaponData, tileData);
    }

    public void TriggerOnWeaponPickup(int index, WeaponData weaponData, TileData tileData)
    {
        OnWeaponPickup?.Invoke(index, weaponData, tileData);
    }

    public void TriggerOnEntityAttackTile(EntityData entityData, WeaponData weaponData, TileData tileData)
    {
        OnEntityAttackTile?.Invoke(entityData, weaponData, tileData);
    }

    public void TriggerOnEntityResourceChange(EntityData entityData)
    {
        OnEntityResourceChange?.Invoke(entityData);
    }

    public void TriggerOnEnemyInspect(EnemyData enemyData)
    {
        OnEntityInspect?.Invoke(enemyData);
    }

    public void TriggerOnEnemyIntent(EnemyData enemyData)
    {
        OnEnemyIntent?.Invoke(enemyData);
    }

    public void TriggerOnGameOver(int score)
    {
        OnGameOver?.Invoke(score);
    }
}
