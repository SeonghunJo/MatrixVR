using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class stores the number of kills required before a particular weapon upgrade is available
/// </summary>
[Serializable]
public class ROTD_WeaponUpgrade
{
    /// <summary>
    /// The number of kills required for this weapon to be available
    /// </summary>
    public int totalKillThreshold;

    /// <summary>
    /// The type of weapon for this upgrade
    /// </summary>
    public ROTD_Weapon.WEAPON_TYPE weaponType;
}