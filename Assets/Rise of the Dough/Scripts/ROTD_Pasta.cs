using UnityEngine;
using System.Collections;
using SmoothMoves;

/// <summary>
/// This class controls the enemies of the game
/// </summary>
public class ROTD_Pasta : MonoBehaviour 
{
    /// <summary>
    /// Internal reference to the game manager
    /// </summary>
    private ROTD_GameManager _gameManager;

    /// <summary>
    /// Internal cache of the transform
    /// </summary>
    private Transform _thisTransform;

    /// <summary>
    /// Internal timer countdown until the pasta checks on the chef's position
    /// </summary>
    private float _checkDirectionTimeLeft;

    /// <summary>
    /// Internal cache of the offset of the pasta from the chef
    /// </summary>
    private Vector3 _offset;

    /// <summary>
    /// Direction to move
    /// </summary>
    private Vector3 _moveDirection;

    /// <summary>
    /// Internal cache of the pasta's position
    /// </summary>
    private Vector3 _position;

    /// <summary>
    /// Internal cache of the pasta's rotation
    /// </summary>
    private Vector3 _localEulerAngles;

    /// <summary>
    /// Pasta's state
    /// </summary>
    private STATE _state;

    /// <summary>
    /// Direction from which the pasta was hit
    /// </summary>
    private float _hitXDirection;

    /// <summary>
    /// Internal countdown timer of when the pasta can move again
    /// </summary>
    private float _waitingToMoveTimeLeft;

    /// <summary>
    /// The amount of health left in the pasta
    /// </summary>
	private float _healthLeft;

    /// <summary>
    /// Internal reference to the live animation of the pasta
    /// </summary>
    private BoneAnimation _liveAnimation;

    /// <summary>
    /// Internal reference to the trail left by the pasta
    /// </summary>
    private Transform _trailTransform;

    /// <summary>
    /// Possible states of the pasta
    /// </summary>
    public enum STATE
    {
        Initializing,
        Spawning,
        WaitingToMove,
        Moving,
        Hitting,
        Attacking,
		Dying,
		Dead
    }

    /// <summary>
    /// Possible types of pasta
    /// </summary>
    public enum TYPE
    {
        Pizza
    }

    /// <summary>
    /// The type of this pasta
    /// </summary>
    public TYPE pastaType;

    /// <summary>
    /// Reference to the prefab of the live animation
    /// </summary>
    public GameObject liveAnimationPrefab;

    /// <summary>
    /// Reference to the particle system that leaves a trail
    /// </summary>
    public ParticleSystem trailParticles;

    /// <summary>
    /// Z position of the trail so that it is always behind foreground objects
    /// </summary>
    public float trailZPosition;
	
    /// <summary>
    /// Reference to the gameobject that contains colliders
    /// </summary>
	public GameObject colliderGameObject;

    /// <summary>
    /// Sound of the pasta getting hit
    /// </summary>
	public AudioSource hitSound;

    /// <summary>
    /// Sound of the pasta dying
    /// </summary>
	public AudioSource splatSound;

    /// <summary>
    /// Sound of the pasta attacking the chef
    /// </summary>
	public AudioSource attackSound;
	
    /// <summary>
    /// Initial health of the pasta
    /// </summary>
	public float health;

    /// <summary>
    /// Amount of damage the pasta can do
    /// </summary>
	public float damage;

    /// <summary>
    /// The speed the pasta travels at
    /// </summary>
    public float moveSpeed;

    /// <summary>
    /// The minimum attack range in the x and y axis
    /// </summary>
    public Vector2 minAttackRange;
    
    /// <summary>
    /// The minimum and maximum pitch for the attack sound (for variety)
    /// </summary>
    public Vector2 minMaxAttackSoundPitch;
    
    /// <summary>
    /// The amount of time before the pasta can move after stopping or getting hit
    /// </summary>
    public float waitToMoveTime;

    /// <summary>
    /// The minimum and maximum check direction times
    /// A random value will be picked between the x and y values
    /// Stored as a vector2 instead of two separate variables for simplicity
    /// </summary>
    public Vector2 minMaxCheckDirectionTime;

    /// <summary>
    /// The speed the pasta flies at when hit
    /// </summary>
    public float hitMoveSpeed;

    /// <summary>
    /// State of the pasta
    /// </summary>
    public STATE State
    {
		get
		{
			return _state;
		}
        set
        {
            // only uodate the state if it has changed
            if (_state != value)
            {
                _state = value;

                switch (_state)
                {
                    case STATE.Spawning:

                        // the pasta is coming alive,
                        // turn off the trail particles and play the Spawn animation

                        trailParticles.enableEmission = false;
                        _liveAnimation.Play("Spawn");
                        break;

                    case STATE.WaitingToMove:

                        // the pasta is waiting to move,
                        // turn off the trail particles, set the wait to move countdown, and play the Stand animation

                        trailParticles.enableEmission = false;
#if UNITY_3_5
                		colliderGameObject.active = true;
#else
                        colliderGameObject.SetActive(true);
#endif
                        _waitingToMoveTimeLeft = waitToMoveTime;
                        _liveAnimation.CrossFade("Stand");
                        break;

                    case STATE.Moving:

                        // the pasta is moving,
                        // turn on the trail particles and play the Move animation

                        trailParticles.enableEmission = true;
                        _liveAnimation.CrossFade("Move");
                        break;

                    case STATE.Hitting:

                        // the pasta is being hit,
                        // turn off the trail particles and play the Hit animation

                        trailParticles.enableEmission = false;
                        _liveAnimation.Play("Hit");
                        break;

                    case STATE.Attacking:

                        // the pasta is attacking,
                        // turn off the trail particles and play the Attack animation

                        trailParticles.enableEmission = false;
                        _liveAnimation.Play("Attack");
                        break;

                    case STATE.Dying:

                        // the pasta is dying,
                        // play the splat sound, activate the splat animation, 
                        // stop the live animation, deactivate the animation gameobject, 
                        // turn off the trail particles, and turn off the collider
					
						splatSound.Play();

                        _gameManager.fxManager.Activate(ROTD_FX.FX_TYPE.Pizza_Splat, _thisTransform.position, (int)UnityEngine.Random.Range(-1.0f, 1.0f), "");
                        _liveAnimation.Stop();
#if UNITY_3_5
                        _liveAnimation.gameObject.SetActiveRecursively(false);
#else
                        _liveAnimation.gameObject.SetActive(false);
#endif
                        trailParticles.enableEmission = false;
					
#if UNITY_3_5
						colliderGameObject.active = false;
#else
                        colliderGameObject.SetActive(false);
#endif
                        break;
					
					case STATE.Dead:

                        // the pasta is dead,
                        // deactivate the gameobject

#if UNITY_3_5
						gameObject.active = false;
#else
                        gameObject.SetActive(false);
#endif

                        
                        // disable other elements in case we jump straight to dead (like on a reset)

#if UNITY_3_5
                        _liveAnimation.gameObject.SetActiveRecursively(false);
#else
                        _liveAnimation.gameObject.SetActive(false);
#endif
                        trailParticles.enableEmission = false;
#if UNITY_3_5
                        colliderGameObject.active = false;
#else
                        colliderGameObject.SetActive(false);
#endif

                        break;
                }
            }
        }
    }

    /// <summary>
    /// Set up the pasta from the Pasta Manager
    /// </summary>
    /// <param name="manager">Reference to the game manager</param>
    public void Initialize(ROTD_GameManager manager)
    {
        // Set the state to initializing
        State = STATE.Initializing;

        // cache some references and perform calculations
		_gameManager = manager;
        _thisTransform = transform.transform;
        _trailTransform = trailParticles.transform;

        // instantiate the live animation
        GameObject go;
        go = (GameObject)Instantiate(liveAnimationPrefab, Vector3.zero, Quaternion.identity);
        go.transform.parent = _thisTransform;
        _liveAnimation = go.GetComponent<BoneAnimation>();

        // register the live animation's delegates
        _liveAnimation.RegisterUserTriggerDelegate(LiveUserTrigger);
        _liveAnimation.RegisterColliderTriggerDelegate(LiveColliderTrigger);

        // start out dead
        State = STATE.Dead;
    }
	
    /// <summary>
    /// Set the pasta as active and initialize it's location
    /// </summary>
	public void ReSpawn()
	{
        // activate game objects
#if UNITY_3_5
		gameObject.active = true;
        _liveAnimation.gameObject.SetActiveRecursively(true);
#else
        gameObject.SetActive(true);
        _liveAnimation.gameObject.SetActive(true);
#endif

        // initialize variables
	    _checkDirectionTimeLeft = 0;
        _healthLeft = health;

        // get the spawn position

        bool fromOven = false;
		_thisTransform.position = _gameManager.pastaManager.GetRandomRespawnPosition(out fromOven);

        if (fromOven)
        {
            _localEulerAngles = _liveAnimation.mLocalTransform.localEulerAngles;
            _localEulerAngles.y = 180.0f;
            _liveAnimation.mLocalTransform.localEulerAngles = _localEulerAngles;

            // Spawn if from oven
            State = STATE.Spawning;
        }
        else
        {
            // Wait to move if not from oven
            State = STATE.WaitingToMove;
        }
	}
	
    /// <summary>
    /// Called every frame
    /// </summary>
    public void FrameUpdate()
    {
        // if spawning, jump out
        if (_state == STATE.Spawning)
            return;

        // if dying
		if (_state == STATE.Dying)
		{
			if (!splatSound.isPlaying)
			{
                // set to dead if the splat sound is done playing
				State = STATE.Dead;
			}
			
            // jump out
			return;
		}
		
        // get the offset from the chef
        _offset = (_gameManager.chef.Position - _thisTransform.position);

        // get the rotation of the pasta based on its offset from the chef
        _localEulerAngles = _liveAnimation.mLocalTransform.localEulerAngles;
        _localEulerAngles.y = (_offset.x > 0 ? 180.0f : 0);
        _liveAnimation.mLocalTransform.localEulerAngles = _localEulerAngles;

        switch (_state)
        {
            case STATE.WaitingToMove:

                // pasta is waiting to move, so count down the timer

                _waitingToMoveTimeLeft -= Time.deltaTime;
                if (_waitingToMoveTimeLeft <= 0)
                {
                    // countdown expired

                    if (!InAttackRange(_offset))
                    {
                        // out of attack range, so just move

                        State = STATE.Moving;
                    }
                }

                return;

            case STATE.Hitting:

                // move the pasta away from where it was hit by the player
                _position = _thisTransform.position;
                _position.x += (_hitXDirection * Time.deltaTime);

                // keep the pasta within the room bounds. This doesn't conform
                // to the room's apparant 3D shape, but it is close enough for
                // this minigame.
                _position.x = Mathf.Clamp(_position.x, _gameManager.room.roomXBoundsMinMax.x, _gameManager.room.roomXBoundsMinMax.y);

                _thisTransform.position = _position;
                break;

            case STATE.Attacking:

                // pasta is attacking
                 
                // count down the check direction timer

                _checkDirectionTimeLeft -= Time.deltaTime;
                if (_checkDirectionTimeLeft <= 0)
                {
                    // countdown expired

                    if (!InAttackRange(_offset))
                    {
                        // out of attack range, so just wait to move

                        State = STATE.WaitingToMove;
                    }

                    // set the check direction countdown timer randomly between the set values
                    _checkDirectionTimeLeft = UnityEngine.Random.Range(minMaxCheckDirectionTime.x, minMaxCheckDirectionTime.y);
                }
                break;

            case STATE.Moving:

                // pasta is moving

                // count down the check direction timer

                _checkDirectionTimeLeft -= Time.deltaTime;
                if (_checkDirectionTimeLeft <= 0)
                {
                    // countdown expired

                    // move in the direction of the offset from the chef
                    _moveDirection = _offset;

                    if (InAttackRange(_offset))
                    {
                        // if in attack range, stop moving
                        _moveDirection = Vector3.zero;
                    }
                    else
                    {
                        // out of attack range, so normalize the move direction
                        _moveDirection.y = 0;
                        _moveDirection.Normalize();
                    }

                    // reset the check direction countdown timer
                    _checkDirectionTimeLeft = UnityEngine.Random.Range(minMaxCheckDirectionTime.x, minMaxCheckDirectionTime.y);
                }

                // if the move direction is not empty
                if (_moveDirection != Vector3.zero)
                {
                    // move in the direction stored
                    _position = _thisTransform.position;
                    _position += (_moveDirection * moveSpeed * Time.deltaTime);

                    // make sure the pasta stays on the 45 degree slope by forcing the y value to the z value
                    _position.y = _position.z;
                    _thisTransform.position = _position;
                }
                break;
        }

        if (InAttackRange(_offset) && _state != STATE.Hitting)
        {
            // if in attack range and not getting hit, then attack

            State = STATE.Attacking;
        }
        else if (_state == STATE.Attacking)
        {
            // else if attacking, then wait to move

            State = STATE.WaitingToMove;
        }

        // set the trail Z position
        _position = _trailTransform.position;
        _position.z = trailZPosition;
        _trailTransform.position = _position;
    }

    /// <summary>
    /// Pasta gets hit
    /// </summary>
    /// <param name="collisionPoint">Location of the collision impact</param>
    /// <param name="damage">Amount of damage from the hit</param>
    public void Hit(Vector3 collisionPoint, float damage)
    {
        // if not already being hit
        if (_state != STATE.Hitting)
        {
            // play the hit sound
			hitSound.Play();

            // Activate some sauce FX for gore
            _gameManager.fxManager.Activate(ROTD_FX.FX_TYPE.Sauce, _thisTransform.position, (int)(_thisTransform.position.x - _gameManager.chef.Position.x), "");
			
            // damage the pasta
			_healthLeft -= damage;
            if (_healthLeft <= 0)
            {
                // the pasta ran out of health, so it is now dying

                State = STATE.Dying;

                // show a score and update the total score
                _gameManager.AddScore(_thisTransform.position);
            }
            else
            {
                // pasta still has some life left, so determine which direction it needs to fly and set its state to Hitting

                _hitXDirection = (collisionPoint.x > _thisTransform.position.x ? -hitMoveSpeed : hitMoveSpeed);
                State = STATE.Hitting;
            }
        }
    }
	
    /// <summary>
    /// Collider Trigger delegate for the live animation
    /// </summary>
    /// <param name="ctEvent">Event sent by the animation</param>
	public void LiveColliderTrigger(ColliderTriggerEvent ctEvent)
	{
        // if the collider event was an attack by the pasta
		if (ctEvent.tag == "Attack")
		{
            // only process enter event types (not stay or exit)
            if (ctEvent.triggerType == ColliderTriggerEvent.TRIGGER_TYPE.Enter)
            {
                // randomize the pitch of the attack sound for some variation
                attackSound.pitch = UnityEngine.Random.Range(minMaxAttackSoundPitch.x, minMaxAttackSoundPitch.y);
                
                // play the attack sound
                attackSound.Play();
				
                // make the chef take damage
                _gameManager.chef.Hit(damage);
            }
		}
	}

    /// <summary>
    /// User trigger delegate for the live animation
    /// </summary>
    /// <param name="utEvent">Event sent by the animation</param>
    public void LiveUserTrigger(UserTriggerEvent utEvent)
    {
        if (utEvent.tag == "Hit Finished")
        {
            // if the event was a finished hit, then set the pasta
            // to a waiting to move state

            State = STATE.WaitingToMove;
        }
        else if (utEvent.tag == "Killable")
        {
            // if the event was a killable event, then make the collider active
            // (the collider starts out inactive when the pasta is spawning)

#if UNITY_3_5
            colliderGameObject.active = true;
#else
            colliderGameObject.SetActive(true);
#endif
        }
        else if (utEvent.tag == "Spawned")
        {
            // if the event is that the pasta has spawned, then
            // set its state to waiting to move

            State = STATE.WaitingToMove;
        }
	}

    /// <summary>
    /// Determines if an offset is within this pasta's attack range.
    /// Note that we could use vector magnitude, but because of the 2.5D nature of
    /// this minigame, the x and y offsets respond better when they are different values.
    /// </summary>
    /// <param name="offset">Offset from an object</param>
    /// <returns>True if in range</returns>
    private bool InAttackRange(Vector3 offset)
    {
        return (Mathf.Abs(offset.x) <= minAttackRange.x && Mathf.Abs(offset.y) <= minAttackRange.y);
    }
}
