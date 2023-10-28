using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class WeaponGenerator : ScriptableObject
{
    public WeaponData[] possibleWeapons;

    public WeaponData GenerateWeapon()
    {
        if (possibleWeapons == null || possibleWeapons.Length == 0)
            return null;

        return possibleWeapons[Random.Range(0, possibleWeapons.Length)].Copy();
    }
}
