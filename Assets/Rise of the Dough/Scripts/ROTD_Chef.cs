using UnityEngine;
using System.Collections;
using SmoothMoves;

/// <summary>
/// The Chef class controls the player's character.
/// </summary>
public class ROTD_Chef : MonoBehaviour 
{
    /// <summary>
    /// Internal countdown of how long the chef has been idle
    /// </summary>
	private float _idleAnimationTimeRemaining;

    /// <summary>
    /// Internal cache of the local angles of the chef (direction)
    /// </summary>
	private Vector3 _localEulerAngles;

    /// <summary>
    /// Internal cache of the direction to move
    /// </summary>
	private Vector3 _moveDirection;

    /// <summary>
    /// Internal cache of the controller component
    /// </summary>
	private Transform _controllerTransform;

    /// <summary>
    /// Internal cache of the chef's position
    /// </summary>
	private Vector3 _position;

    /// <summary>
    /// Internal cache of any pasta we have hit
    /// </summary>
    private ROTD_Pasta _hitPasta;

    /// <summary>
    /// Internal cache of how much health the chef has left
    /// </summary>
	private float _healthLeft;

    /// <summary>
    /// The start position of the chef for resetting
    /// </summary>
    private Vector3 _startPosition;

    /// <summary>
    /// The start rotation for the chef for resetting
    /// </summary>
    private Quaternion _startRotation;

    /// <summary>
    /// The current weapon that the player is carrying
    /// </summary>
    private ROTD_WeaponProperties _currentWeapon;

    /// <summary>
    /// Reference to the game manager
    /// </summary>
    public ROTD_GameManager gameManager;

    /// <summary>
    /// Reference to the chef's animation
    /// </summary>
	public BoneAnimation boneAnimation;

    /// <summary>
    /// Reference to the Character Controller physics component
    /// </summary>
	public CharacterController controller;
	
    /// <summary>
    /// The minimum time the chef can be idle before an idle animation can occur
    /// </summary>
	public float minIdleAnimationTime;

    /// <summary>
    /// The maximum time the chef can be idle before an idle animation can occur
    /// </summary>
	public float maxIdleAnimationTime;

    /// <summary>
    /// The probability that an idle animation will play when the idle countdown has expired
    /// </summary>
	public float idleAnimationProbability;
	
    /// <summary>
    /// The speed of the chef
    /// </summary>
    public float speed;

    /// <summary>
    /// The initial health of the chef
    /// </summary>
	public float health;

    /// <summary>
    /// States the chef can be in
    /// </summary>
	public enum STATE
	{
		Standing,
		Running,
		Attacking,
        Dying,
        PickingUpWeapon
	}
	
    /// <summary>
    /// Current state of the chef
    /// </summary>
	public STATE State { get; set; }

    /// <summary>
    /// Current position of the chef
    /// </summary>
    public Vector3 Position
    {
        get
        {
            return _controllerTransform.position;
        }
    }
	
    /// <summary>
    /// Called once before other scripts in the scene
    /// </summary>
	void Awake()
	{
        // cache the controller's transform for quicker lookup
		_controllerTransform = controller.transform;

        // initialize the weapon 
        _currentWeapon = new ROTD_WeaponProperties();
        _currentWeapon.weaponType = ROTD_Weapon.WEAPON_TYPE.RollingPin;
        _currentWeapon.damage = gameManager.weapon.GetWeaponDamage(_currentWeapon.weaponType);

        // store the start position and rotation for resetting
        _startPosition = _controllerTransform.position;
        _startRotation = _controllerTransform.localRotation;

        // make sure the probability is in the range between zero and one
		idleAnimationProbability = Mathf.Clamp01(idleAnimationProbability);
		
        // register the collider and user delegates
		boneAnimation.RegisterColliderTriggerDelegate(ColliderTrigger);
		boneAnimation.RegisterUserTriggerDelegate(UserTrigger);
	}
	
    /// <summary>
    /// Called every frame from the game manager
    /// </summary>
	public void FrameUpdate () 
	{
        // if the chef is dying or picking up the weapon, then jump out
        if (State == STATE.Dying || State == STATE.PickingUpWeapon)
            return;

        // check the state of the chef first
		CheckState();		
		
        // count down the idle timer
		_idleAnimationTimeRemaining -= Time.deltaTime;
		if (_idleAnimationTimeRemaining <= 0)
		{
            // the idle timer has expired

            // check to see if the idle animation will play
			if (UnityEngine.Random.Range(0, 1.0f) <= idleAnimationProbability)
			{
                // make sure the chef is standing
				if (State == STATE.Standing)
				{
                    // randomly pick an idle animation
					int idleAnimation = UnityEngine.Random.Range(0, 2);
					
					switch (idleAnimation)
					{
					case 0:
                        // play the scratch animation
						boneAnimation.Play("Scratch");
						break;
						
					case 1:
                        // play the taunt animation
						boneAnimation.CrossFade("Taunt");
						break;
					}
					
				}
			}
				
            // reset the idle animation countdown timer
			ResetIdleAnimationCountdown();
		}
	}
	
    /// <summary>
    /// Collider Trigger delegate called when a collider in the chef interacts with the world
    /// </summary>
    /// <param name="colliderTriggerEvent">Event captured when a collision occurs</param>
	public void ColliderTrigger(ColliderTriggerEvent colliderTriggerEvent)
	{
        // check to see if the event is the enter type (we don't want to process constant
        // collisions or exits)
		if (colliderTriggerEvent.triggerType == ColliderTriggerEvent.TRIGGER_TYPE.Enter)
		{
            // check to see if the tag of the event is "Attack"
			if (colliderTriggerEvent.tag == "Attack")
			{
                // check to see what object we collided with

				if (colliderTriggerEvent.otherCollider.name == "Refrigerator")
				{
                    // play the sound of hitting the fridge
					gameManager.soundFXManager.Play("hit_refrigerator");
				}
                else if (colliderTriggerEvent.otherCollider.name == "Oven")
                {
                    // play the sound of hitting the oven
                    gameManager.soundFXManager.Play("hit_metal");
                }
                else
                {
                    // we hit a pasta

                    _hitPasta = colliderTriggerEvent.otherCollider.gameObject.transform.parent.GetComponent<ROTD_Pasta>();
                    if (_hitPasta != null)
                    {
                        // send a notification to the pasta that it was hit and how hard
                        _hitPasta.Hit(colliderTriggerEvent.otherColliderClosestPointToBone, _currentWeapon.damage);
                    }
                }
			}
		}
	}
	
    /// <summary>
    /// Resets the idle countdown timer
    /// </summary>
	private void ResetIdleAnimationCountdown()
	{
        // randomly set the remaining time between the minimum and maximum idle animation times
		_idleAnimationTimeRemaining = UnityEngine.Random.Range(minIdleAnimationTime, maxIdleAnimationTime);
	}
	
    /// <summary>
    /// Checks the state of the chef so that when he is done attacking he will stand
    /// </summary>
	public void CheckState()
	{
        // if attacking
		if (
			!(
			boneAnimation.IsPlaying("QuickAttack_01")
			||
			boneAnimation.IsPlaying("QuickAttack_02")
			||
			boneAnimation.IsPlaying("QuickAttack_03")
			)
			&&
			State == STATE.Attacking)
		{
            // stand
			State = STATE.Standing;
		}		
	}
	
    /// <summary>
    /// Moves the chef
    /// </summary>
    public void Move()
    {
        // if the state is picking up the weapon, then jump out
        if (State == STATE.PickingUpWeapon)
            return;

        // if the move direction is not empty
        if (_moveDirection != Vector3.zero)
        {
            // move the character controller in the direction calculated by the Input Manager
            controller.Move(_moveDirection * Time.deltaTime);
			
			// make sure the chef stays on the 45 degree slope by forcing the y value to the z value
			_position = _controllerTransform.position;
			_position.y = _position.z;
			_controllerTransform.position = _position;
        }
    }	
	
    /// <summary>
    /// Gets the offset of a position from the chef
    /// </summary>
    /// <param name="position">Other world position</param>
    /// <returns>Offset value</returns>
	public Vector3 OffsetFromPosition(Vector3 position)
	{
		// Note that we are using the y position value for the z position.
		// This is intentional and lets us move the player along a 45 degree plane.
		// The purpose of this is to make objects at the bottom of the screen be in front of objects
		// higher up.
        return (new Vector3(position.x, position.y, position.y) - _controllerTransform.position);
	}
	
    /// <summary>
    /// Makes the chef face a direction
    /// </summary>
    /// <param name="direction">Direction to face (looking at the sign of the x component)</param>
	public void FaceDirection(Vector3 direction)
	{
        // set the local Y angle to 180 degrees if the direction is greater than zero, else keep the Y angle at zero.
        _localEulerAngles = boneAnimation.mLocalTransform.localEulerAngles;
        _localEulerAngles.y = (direction.x > 0 ? 180.0f : 0);
        boneAnimation.mLocalTransform.localEulerAngles = _localEulerAngles;
	}
	
    /// <summary>
    /// Make the chef run in a direction
    /// </summary>
    /// <param name="direction">Direction to run</param>
    /// <param name="playAnimation">Whether or not to play the run animation sequence</param>
	public void Run(Vector3 direction, bool playAnimation)
	{
        // set the move direction based on the normalized direction times the chef's speed
        _moveDirection = direction.normalized * speed;

        // set the chef's state to running
		State = STATE.Running;		

        // if we are to play the animation, then call the sequence "Stand_To_Run", then "Run"
        // we do this to avoid odd "popping" effects of texture switching of the leg. "Stand_To_Run"
        // is an intermediate animation that gets the leg to the position it needs to be in
        // for the "Run" animation
		if (playAnimation)
			PlayBackToBack("Stand_To_Run", "Run");
	}
	
    /// <summary>
    /// Makes the chef stand
    /// </summary>
    /// <param name="playAnimation">Whether or not to play the animation</param>
	public void Stand(bool playAnimation)
	{
        // zero out our move direction
        _moveDirection = Vector3.zero;

        // set our chef to the standing state
        State = ROTD_Chef.STATE.Standing;		

        // if we are to play the animation, then call the sequence "Run_To_Stand", then "Stand".
        // This avoids the "popping" effects of texture switching of the leg.
		if (playAnimation)
			PlayBackToBack("Run_To_Stand", "Stand");
	}
	
    /// <summary>
    /// Makes the chef attack
    /// </summary>
    /// <param name="worldPosition">location to attack</param>
	public void Attack(Vector3 worldPosition)
	{
        // set the chef's state to attacking
        State = ROTD_Chef.STATE.Attacking;

        // make the chef face a direction
        FaceDirection(OffsetFromPosition(worldPosition));

        // play one of the attack animations randomly
        // don't play attack 3 for cleaver since it looks kind of silly to poke with a cleaver :)
        boneAnimation.Play("QuickAttack_" + UnityEngine.Random.Range(1, (_currentWeapon.weaponType == ROTD_Weapon.WEAPON_TYPE.Cleaver ? 3 : 4)).ToString("00"));
    }
	
    /// <summary>
    /// Shortcut to calling play and playqueued on two animations
    /// </summary>
    /// <param name="animation">First animation to play</param>
    /// <param name="animation2">Animation to queue up</param>
	public void PlayBackToBack(string animation, string animation2)
	{
        boneAnimation.Play(animation);
		boneAnimation.PlayQueued(animation2);
	}	
	
    /// <summary>
    /// User trigger delegate called when the animation sends a user trigger
    /// </summary>
    /// <param name="userTriggerEvent">User trigger event passed by the animation</param>
	public void UserTrigger(SmoothMoves.UserTriggerEvent userTriggerEvent)
	{
        // determine what user trigger was fired

		if (userTriggerEvent.tag == "play swish")
		{
            // play the swish sound
            gameManager.soundFXManager.Play("swish");
		}
		else if (userTriggerEvent.tag == "turn on colors")
		{
            // set the color processing on.
            // this saves us from having to constantly process colors (costly)
            // instead only processing when necessary
			boneAnimation.updateColors = true;
		}
		else if (userTriggerEvent.tag == "turn off colors")
		{
            // set the color processing off
            // this saves us from having to constantly process colors (costly)
            // instead only processing when necessary
            boneAnimation.updateColors = false;
		}
        else if (userTriggerEvent.tag == "Done Picking Up Weapon")
        {
            // if we are done picking up the weapon, then set the state back to standing
            State = STATE.Standing;
        }
	}
	
    /// <summary>
    /// Chef was hit
    /// </summary>
    /// <param name="damage">Amount of damage of the hit</param>
	public void Hit(float damage)
	{
        // decrease health
		_healthLeft -= damage;

        // update the GUI health bar
        gameManager.guiManager.chefHealthBar.Progress = (_healthLeft / health);
        
        // if the chef is out of health
        if (_healthLeft <= 0)
		{
            if (State != STATE.Dying)
            {
                boneAnimation.Play("Dying");
                State = STATE.Dying;

                gameManager.State = ROTD_GameManager.STATE.GameOver;
            }
		}
	}

    /// <summary>
    /// Resets the chef back to the initial state
    /// </summary>
    public void ResetToStart()
    {
        // set the position and rotation to the start positions
        _controllerTransform.position = _startPosition;
        _controllerTransform.localRotation = _startRotation;

        // set the amount of health left to the initial health
        _healthLeft = health;

        // reset the weapon to the rolling pin
        _currentWeapon.weaponType = ROTD_Weapon.WEAPON_TYPE.RollingPin;
        _currentWeapon.damage = gameManager.weapon.GetWeaponDamage(_currentWeapon.weaponType);

        // reset the weapon texture back to the rolling pin
        boneAnimation.RestoreTextures();

        // reset the idle animation countdown timer
        ResetIdleAnimationCountdown();

        // stop all animations and play the stand animation
        State = STATE.Standing;
        boneAnimation.Stop();
        boneAnimation.Play("Stand");
    }

    /// <summary>
    /// Switches the chef's weapon
    /// </summary>
    public void PickUpWeapon(ROTD_Weapon.WEAPON_TYPE weaponType)
    {
        // switch the state to picking up the weapon
        State = STATE.PickingUpWeapon;

        // stop the movement
        _moveDirection = Vector3.zero;

        // set the current weapon
        _currentWeapon.weaponType = weaponType;
        _currentWeapon.damage = gameManager.weapon.GetWeaponDamage(_currentWeapon.weaponType);

        switch (_currentWeapon.weaponType)
        {
            case ROTD_Weapon.WEAPON_TYPE.Knife:
                // change the texture of all animation clips and all bones in our animation
                // from the rolling pin texture to the knife texture
                boneAnimation.SwapTexture("Weapons", "rolling_pin", "Weapons", "knife");
                break;

            case ROTD_Weapon.WEAPON_TYPE.Cleaver:
                // change the texture of all animation clips and all bones in our animation
                // from the rolling pin texture to the knife texture
                //
                // Note that we don't change from the knife to cleaver as this would get complicated and 
                // would require that you keep track of the current texture. Instead you can just
                // reference the original texture, which in this case is the rolling pin.
                boneAnimation.SwapTexture("Weapons", "rolling_pin", "Weapons", "cleaver");
                break;
        }

        // play the animation to show the weapon has been picked up
        PlayBackToBack("Pickup Weapon", "Stand");
    }
}
