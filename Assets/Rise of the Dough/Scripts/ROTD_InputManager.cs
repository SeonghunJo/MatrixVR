using UnityEngine;
using System.Collections;
using SmoothMoves;

/// <summary>
/// This class controls the movement and attacking of the chef. It uses a touchpad class to capture
/// touches on the screen (or with a mouse).
/// </summary>
public class ROTD_InputManager : MonoBehaviour 
{
    /// <summary>
    /// Whether we are currently touching the screen
    /// </summary>
    private bool _touchingScreen;

    /// <summary>
    /// The time when we began our touch
    /// </summary>
    private float _touchDownTime;

    /// <summary>
    /// The location where we began our touch
    /// </summary>
    private Vector3 _touchDownPosition;

    /// <summary>
    /// Internal cached value of the maximum tap move distance squared.
    /// This saves us from having to square the value each frame
    /// </summary>
    private float _maxTapMoveDistanceSquared;

    /// <summary>
    /// The amount of time that has elapsed since we touched the screen
    /// </summary>
    private float _touchTime;

    /// <summary>
    /// Internal cached value of the touch distance squared.
    /// This saves us from having to square the value each frame
    /// </summary>
    private float _touchDistanceSquared;

    /// <summary>
    /// Internal cached value of the maximum stopping distance squared
    /// </summary>
    private float _maxStoppingDistanceSquared;

    /// <summary>
    /// Internal cached value of the minimum starting distance squared
    /// </summary>
    private float _minStartingDistanceSquared;

    /// <summary>
    /// Reference to the game manager
    /// </summary>
    public ROTD_GameManager gameManager;

    /// <summary>
    /// Reference to the touchpad class to capture the input
    /// </summary>
    public ROTD_TouchPad touchPad;

    /// <summary>
    /// Reference to the buy now touchpad at the start of the game
    /// </summary>
    public ROTD_TouchPad buyNowTouchPad;

    /// <summary>
    /// The maximum distance we can move before the tap is turned into a press
    /// </summary>
	public float maxTapMoveDistance;

    /// <summary>
    /// The minimum time we can press before a tap is turned into a press
    /// </summary>
    public float minPressAndHoldTime;

    /// <summary>
    /// The maximum stopping distance between our touch and the chef.
    /// If the chef gets within this distance, then he will stop moving
    /// </summary>
    public float maxStoppingDistance;

    /// <summary>
    /// The minimum starting distance between our touch and the chef.
    /// The chef will only move when the distance is greater than this value
    /// </summary>
    public float minStartingDistance;

    /// <summary>
    /// Called once before Start
    /// </summary>
    void Awake()
    {
        // cache our squared values so we don't have to square them each frame.
        // squaring the distance values allows us to do vector math without 
        // taking a square root (which can be costly).

        _maxTapMoveDistanceSquared = maxTapMoveDistance * maxTapMoveDistance;
        _maxStoppingDistanceSquared = maxStoppingDistance * maxStoppingDistance;
        _minStartingDistanceSquared = minStartingDistance * minStartingDistance;
    }

    /// <summary>
    /// Called at the start of the scene
    /// </summary>
	void Start () 
    {
        // set up the buy now touch pad delegate
        buyNowTouchPad.SetTouchPadDelegate(BuyNow_TouchPadDelegate);

        // set up the touch pad delegate
        touchPad.SetTouchPadDelegate(TouchPadDelegate);
	}

    /// <summary>
    /// Called every frame from the game manager
    /// </summary>
    public void FrameUpdate()
    {
#if UNITY_3_5
        if (buyNowTouchPad.gameObject.active)
        {
            // update the buy now touchpad for mouse captures
            buyNowTouchPad.FrameUpdate();
        }
#else
        if (buyNowTouchPad.gameObject.activeSelf)
        {
            // update the buy now touchpad for mouse captures
            buyNowTouchPad.FrameUpdate();
        }
#endif

        // update the touchpad for mouse captures
        touchPad.FrameUpdate();

        // move the chef
        gameManager.chef.Move();
    }

    /// <summary>
    /// The delegate that will respond to our touchpad's events for buying smooth moves
    /// </summary>
    /// <param name="touchEvent">The event captured by the touchpad</param>
    /// <param name="screenPosition">The screen position of the event</param>
    /// <param name="guiPosition">The GUI position of the event</param>
    /// <param name="worldPosition">The world position of the event</param>
    public void BuyNow_TouchPadDelegate(ROTD_TouchPad.TOUCH_EVENT touchEvent, Vector2 screenPosition, Vector3 guiPosition, Vector3 worldPosition)
    {
        switch (touchEvent)
        {
            case ROTD_TouchPad.TOUCH_EVENT.TouchDown:

                gameManager.ShowAssetStoreLink();

                break;
        }
    }

    /// <summary>
    /// The delegate that will respond to our touchpad's events
    /// </summary>
    /// <param name="touchEvent">The event captured by the touchpad</param>
    /// <param name="screenPosition">The screen position of the event</param>
    /// <param name="guiPosition">The GUI position of the event</param>
    /// <param name="worldPosition">The world position of the event</param>
    public void TouchPadDelegate(ROTD_TouchPad.TOUCH_EVENT touchEvent, Vector2 screenPosition, Vector3 guiPosition, Vector3 worldPosition)
    {
        // if showing asset store link, don't process regular touch input
        if (gameManager.BuyNow)
        {
            gameManager.BuyNow = false;
            return;
        }

        // if the game is over or the chef is picking up the weapon, then jump out
        if (gameManager.State == ROTD_GameManager.STATE.GameOver || gameManager.chef.State == ROTD_Chef.STATE.PickingUpWeapon)
            return;

        // if the game is waiting for the user to press the screen
        if (gameManager.State == ROTD_GameManager.STATE.WaitingForInput)
        {
            switch (touchEvent)
            {
                case ROTD_TouchPad.TOUCH_EVENT.TouchDown:

                    // user pressed the screen, so now we can play

                    gameManager.State = ROTD_GameManager.STATE.Playing;
                    return;
            }

            return;
        }

        // check the state of the chef before processing any input
        gameManager.chef.CheckState();
		
        switch (touchEvent)
        {
            case ROTD_TouchPad.TOUCH_EVENT.TouchDown:

                // the touchpad received a touch began event,
                // so we capture and store some values

                _touchingScreen = true;
                _touchDownTime = Time.realtimeSinceStartup;
                _touchDownPosition = worldPosition;

                break;

            case ROTD_TouchPad.TOUCH_EVENT.TouchMove:
            case ROTD_TouchPad.TOUCH_EVENT.TouchStationary:

                // the touchpad received a move or stationary touch event
                // (finger / mouse is pressing the screen)

                // store the offset from the chef to the world position of the touch
                Vector3 offset = gameManager.chef.OffsetFromPosition(worldPosition);

                // make the chef face the direction of the touch
                gameManager.chef.FaceDirection(offset);

                // if the chef is not attacking and we are touching the screen
                if (gameManager.chef.State != ROTD_Chef.STATE.Attacking && _touchingScreen)
                {
                    // get the amount of time that has elapsed since we began the touch
                    _touchTime = Time.realtimeSinceStartup - _touchDownTime;
				
                    // if the amount of elapsed time is greater than our minimum press and hold time
                    if (_touchTime >= minPressAndHoldTime)
                    {
                        // if the chef is in a standing state
                        if (gameManager.chef.State == ROTD_Chef.STATE.Standing)
                        {
                            // if the distance from our touch to the chef is greater than the minimum starting distance
                            if (offset.sqrMagnitude >= _minStartingDistanceSquared)
                            {
                                // make the chef run towards the touch
                                gameManager.chef.Run(offset, true);
                            }
                        }
                        else
                        {
                            // chef is not in a standing state

                            // if the distance from the touch to the chef is less than the stopping distance
                            if (offset.sqrMagnitude <= _maxStoppingDistanceSquared)
                            {
                                // make the chef stand still
                                gameManager.chef.Stand(true);
                            }
                            else
                            {
                                // chef is still farther away from the touch than the max stopping distance, so
                                // make him continue to run
                                gameManager.chef.Run(offset, false);
                            }
                        }
                    }
                }
                break;

            case ROTD_TouchPad.TOUCH_EVENT.TouchUp:

                // the touchpad received a touch end event

                // if the chef is not attacking and we are touching the screen
                if (gameManager.chef.State != ROTD_Chef.STATE.Attacking && _touchingScreen)
                {
                    // capture the time since we began touching
                    _touchTime = Time.realtimeSinceStartup - _touchDownTime;

                    // cache the distance of the touch from its initial position
                    _touchDistanceSquared = (_touchDownPosition - worldPosition).sqrMagnitude;

                    // if the touch was a tap (didn't move the touch much and didn't press very long)
                    if (
                        _touchDistanceSquared <= _maxTapMoveDistanceSquared
                        &&
                        _touchTime < minPressAndHoldTime
                        )
                    {
                        // make the chef attack
                        gameManager.chef.Attack(worldPosition);
                    }
                }

                // we are no longer touching the screen
                _touchingScreen = false;

                // if the chef is running
                if (gameManager.chef.State == ROTD_Chef.STATE.Running)
				{
                    // make the chef stand still
                    gameManager.chef.Stand(true);
				}
			
                break;
        }
    }

    /// <summary>
    /// Turns off the buy now touchpad
    /// </summary>
    public void TurnOffBuyNowTouchPad()
    {
#if UNITY_3_5
        buyNowTouchPad.gameObject.active = false;
#else
        buyNowTouchPad.gameObject.SetActive(false);
#endif
    }
}
