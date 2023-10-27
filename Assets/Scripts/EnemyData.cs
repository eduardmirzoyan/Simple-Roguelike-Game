using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Entity/Enemy")]
public class EnemyData : EntityData
{
    [Header("AI")]
    public AI ai;
    public Pathfinder pathfinder;

    [Header("Loot")]
    public WeaponData weaponLoot;

    public override EntityData Copy()
    {
        var copy = base.Copy() as EnemyData;

        copy.ai = ai.Copy();

        if (weaponLoot != null)
            copy.weaponLoot = weaponLoot.Copy();

        copy.pathfinder = new Pathfinder();

        return copy;
    }
}
