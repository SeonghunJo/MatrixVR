using UnityEngine;
using System.Collections;
using SmoothMoves;

/// <summary>
/// The Room class has the bounds of the room and oven door settings
/// </summary>
public class ROTD_Room : MonoBehaviour 
{
    /// <summary>
    /// Internal countdown timer for the oven door to be open
    /// </summary>
    private float _ovenDoorOpenTimeLeft;

    /// <summary>
    /// Reference to the oven door sprite
    /// </summary>
    public SmoothMoves.Sprite ovenDoor;

    /// <summary>
    /// Amount of time the oven door should be open
    /// </summary>
    public float ovenDoorOpenTime;

    /// <summary>
    /// Sound of the oven door opening
    /// </summary>
    public AudioSource ovenDoorOpenSound;

    /// <summary>
    /// Room bounds
    /// </summary>
    public Vector2 roomXBoundsMinMax;

    /// <summary>
    /// Opens the oven
    /// </summary>
    public void OpenOven()
    {
        // turn on the oven door sprite (makes the oven look open)
#if UNITY_3_5
        ovenDoor.gameObject.active = true;
#else
        ovenDoor.gameObject.SetActive(true);
#endif

        // play the oven door open sound
        ovenDoorOpenSound.Play();

        // set the countdown timer
        _ovenDoorOpenTimeLeft = ovenDoorOpenTime;
    }

    /// <summary>
    /// Called every frame from the game manager
    /// </summary>
    public void FrameUpdate()
    {
        // if the countdown timer is running
        if (_ovenDoorOpenTimeLeft > 0)
        {
            _ovenDoorOpenTimeLeft -= Time.deltaTime;
            if (_ovenDoorOpenTimeLeft <= 0)
            {
                // countdown timer expired

                // turn off the oven door sprite (makes the oven door look closed)
#if UNITY_3_5
                ovenDoor.gameObject.active = false;
#else
                ovenDoor.gameObject.SetActive(false);
#endif
            }
        }
    }
}
