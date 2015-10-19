using UnityEngine;
using System.Collections;

/// <summary>
/// This class handles the weapon upgrade in the scene.
/// It also stores some information about the weapons for reference from other classes
/// </summary>
public class ROTD_Weapon : MonoBehaviour 
{
    /// <summary>
    /// The type of the weapon
    /// </summary>
    private WEAPON_TYPE _weaponType;

    /// <summary>
    /// The cached transform of the gameobject
    /// </summary>
    private Transform _thisTransform;

    /// <summary>
    /// The weapon types available
    /// </summary>
    public enum WEAPON_TYPE
    {
        RollingPin,
        Knife,
        Cleaver
    }

    /// <summary>
    /// Reference to the game manager
    /// </summary>
    public ROTD_GameManager gameManager;

    /// <summary>
    /// Reference to the sparkling weapon upgrade
    /// </summary>
    public SmoothMoves.BoneAnimation boneAnimation;

    /// <summary>
    /// List of weapons and their damages
    /// </summary>
    public ROTD_WeaponProperties[] weaponProperties;

    /// <summary>
    /// Sets the weapon upgrade in the scene
    /// </summary>
    public WEAPON_TYPE WeaponType
    {
        set
        {
            _weaponType = value;

            switch (_weaponType)
            {
                case WEAPON_TYPE.Knife:
                    // knife is the original weapon upgrade in the animation, so just restore the textures
                    boneAnimation.RestoreTextures();
                    break;

                case WEAPON_TYPE.Cleaver:
                    // swap the cleaver for the knife in the animations
                    boneAnimation.SwapTexture("Weapon Animation", "knife", "Weapon Animation", "cleaver");
                    break;
            }

            // set the position of the upgrade
            _thisTransform.localPosition = GetWeaponLocation(_weaponType);
        }
    }

    /// <summary>
    /// Called once before other scripts
    /// </summary>
    void Awake()
    {
        // cache the transform for faster lookup
        _thisTransform = this.transform;
    }

    /// <summary>
    /// Occurs when the chef enters the upgrade trigger zone
    /// </summary>
    /// <param name="otherCollider">The collider that triggered this event</param>
    void OnTriggerEnter(Collider otherCollider)
    {
        // make the chef pick up the upgraded weapon
        gameManager.chef.PickUpWeapon(_weaponType);

        gameManager.soundFXManager.Play("tada");

        // turn the upgrade off
        Toggle(false);
    }

    /// <summary>
    /// Set the weapon upgrade back to its initial state
    /// </summary>
    public void ResetToStart()
    {
        Toggle(false);
    }

    /// <summary>
    /// Turn the weapon upgrade on or off
    /// </summary>
    /// <param name="on"></param>
    public void Toggle(bool on)
    {
#if UNITY_3_5
        gameObject.SetActiveRecursively(on);
#else
        gameObject.SetActive(on);
#endif
    }

    /// <summary>
    /// Lookup a weapon's damage based on the inspector values
    /// </summary>
    /// <param name="weaponType">Type of weapon to look up</param>
    /// <returns>Damage value of the weapon</returns>
    public float GetWeaponDamage(WEAPON_TYPE weaponType)
    {
        // loop through each weapon property set in the inspector
        foreach (ROTD_WeaponProperties wp in weaponProperties)
        {
            if (wp.weaponType == weaponType)
            {
                // this is the weapon we are looking for, so we return the damage

                return wp.damage;
            }
        }

        // no weapon found in the property listing, so we return no damage
        return 0;
    }

    /// <summary>
    /// Lookup a weapon's upgrade location based on the inspector values
    /// </summary>
    /// <param name="weaponType">Type of weapon to look up</param>
    /// <returns>upgrade location of the weapon</returns>
    public Vector3 GetWeaponLocation(WEAPON_TYPE weaponType)
    {
        // loop through each weapon property set in the inspector
        foreach (ROTD_WeaponProperties wp in weaponProperties)
        {
            if (wp.weaponType == weaponType)
            {
                // this is the weapon we are looking for, so we return the location

                return wp.location;
            }
        }

        // no weapon found in the property listing, so we return origin
        return Vector3.zero;
    }
}
