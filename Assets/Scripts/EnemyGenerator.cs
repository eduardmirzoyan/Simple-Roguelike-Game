using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnemyGenerator : ScriptableObject
{
    [Header("Enemies")]
    public EnemyData vase;
    public EnemyData chest;
    public EnemyData[] possibleEnemies;

    [Header("Generator")]
    public WeaponGenerator weaponGenerator;

    public EnemyData GenerateEnemy()
    {
        if (possibleEnemies == null || possibleEnemies.Length == 0)
            return null;

        return possibleEnemies[Random.Range(0, possibleEnemies.Length)].Copy() as EnemyData;
    }

    public EnemyData GenerateVase()
    {
        return vase.Copy() as EnemyData;
    }

    public EnemyData GenerateChest()
    {
        var enemy = chest.Copy() as EnemyData;

        // Generate random weapon
        var weapon = weaponGenerator.GenerateWeapon();
        enemy.weaponLoot = weapon;

        return enemy;
    }

}
