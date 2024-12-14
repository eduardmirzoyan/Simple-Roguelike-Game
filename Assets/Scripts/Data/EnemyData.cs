using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Entity/Enemy")]
public class EnemyData : EntityData
{
    [Header("AI")]
    public AI ai;

    [Header("Loot")]
    public WeaponData weaponLoot;

    public WeaponData Weapon
    {
        get
        {
            if (weapons == null || weapons[0] == null)
                throw new System.Exception($"Enemy {name} weapon was not set!");

            return weapons[0];
        }
    }

    public override EntityData Copy()
    {
        var copy = base.Copy() as EnemyData;

        if (ai != null)
            copy.ai = ai.Copy();

        if (weaponLoot != null)
            copy.weaponLoot = weaponLoot.Copy();

        return copy;
    }
}
