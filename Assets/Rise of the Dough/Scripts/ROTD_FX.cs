using UnityEngine;
using SmoothMoves;

/// <summary>
/// The FX class allows multiple objects to be managed by the FXManager class. Each FX has a Dead and Alive state.
/// Each FX has a bone animation that should contain one or many FX_xx animation clips that are played randomly.
/// A user trigger should be included in the bone animation to that sends a "Dead" tag to notify the manager that
/// the FX has completed its animation.
/// </summary>
public class ROTD_FX : MonoBehaviour
{
    /// <summary>
    /// The game manager class that governs the game
    /// </summary>
    protected ROTD_GameManager _gameManager;

    /// <summary>
    /// The current state of this FX
    /// </summary>
    protected STATE _state;

    /// <summary>
    /// Used for centering some objects
    /// </summary>
    protected Vector3 _originalPosition;

    /// <summary>
    /// The cached transform of the FX gameobject
    /// </summary>
    protected Transform _thisTransform;

    /// <summary>
    /// The playing animation clip name
    /// </summary>
    protected string _animationClipName = "";

    /// <summary>
    /// Possible types of the FX
    /// </summary>
    public enum FX_TYPE
    {
        Sauce,
        Pizza_Splat,
        Score
    }

    /// <summary>
    /// The state of the FX
    /// </summary>
    public enum STATE
    {
        Dead,
        Alive
    }

    /// <summary>
    /// The type of this FX
    /// </summary>
    public FX_TYPE fxType;

    /// <summary>
    /// The number of animation clips that can be played (the animation will be randomly chosen)
    /// </summary>
    public int animationVariationCount;

    /// <summary>
    /// A reference to the bone animation for this FX
    /// </summary>
    public BoneAnimation boneAnimation;

    /// <summary>
    /// The public accessor of the state of the FX
    /// </summary>
    public STATE State
    {
        get
        {
            return _state;
        }
        set
        {
            _state = value;

            switch (_state)
            {
                case STATE.Alive:

                    // The FX has been set to alive, so we activate the gameobject(s) and play a random animation clip

#if UNITY_3_5
                    gameObject.SetActiveRecursively(true);
#else
                    gameObject.SetActive(true);
#endif

                    _animationClipName = "FX_" + UnityEngine.Random.Range(0, animationVariationCount).ToString("00");
                    boneAnimation.Play(_animationClipName);
                    break;

                case STATE.Dead:

                    // The FX has been set to dead, so we stop the playing animation clip and deactivate the gameobject(s)

                    if (_animationClipName != "")
                        boneAnimation.Stop(_animationClipName);

#if UNITY_3_5
                    gameObject.SetActiveRecursively(false);
#else
                    gameObject.SetActive(false);
#endif
                    break;
            }
        }
    }

    /// <summary>
    /// This value is overridden in inherited classes
    /// </summary>
    public virtual string Value
    {
        get
        {
            return "";
        }
        set
        {
        }
    }

    /// <summary>
    /// This sets up the animation when it is created by the FXManager
    /// </summary>
    /// <param name="manager"></param>
    public void Initialize(ROTD_GameManager manager)
    {
        // cache the transform for quicker lookup
        _thisTransform = this.transform;

        // store the game manager reference
        _gameManager = manager;

        // register the user trigger delegate that will notify this class that it has
        // finished playing its animation
        boneAnimation.RegisterUserTriggerDelegate(UserTrigger);

        // start the FX in the dead state
        State = STATE.Dead;
    }

    /// <summary>
    /// Resets the FX to a position, direction, and initializes its value
    /// </summary>
    /// <param name="position">The location of the FX</param>
    /// <param name="direction">The direction of the FX</param>
    /// <param name="value">The value of the FX</param>
    public void Reset(Vector3 position, int direction, string value)
    {
        switch (fxType)
        {
            case FX_TYPE.Sauce:
            case FX_TYPE.Pizza_Splat:

                // pizzas and splats will be moved to a background Z position
                // so that they do not overlap foreground elements

                position.z = _gameManager.fxManager.backgroundZPosition;
                break;

            case FX_TYPE.Score:

                // nothing needs to be done for the score since we want it floating
                // in 3D space (in front and behind other objects)

                break;
        }

        // move the FX to the position and set the direction by setting the Y rotation
        _thisTransform.position = position;
        _thisTransform.localEulerAngles = new Vector3(0, (direction > 0 ? 180.0f : 0), 0);
        _originalPosition = position;

        // set the FX to alive
        State = STATE.Alive;

        // set the FX's value (only applicable to inherited classes)
        Value = value;
    }

    /// <summary>
    /// This is the delegate that will be called when user triggers are processed by the animation
    /// </summary>
    /// <param name="utEvent">The user trigger event that occurred</param>
    public void UserTrigger(UserTriggerEvent utEvent)
    {
        if (utEvent.tag == "Dead")
        {
            // The user trigger's tag was "Dead", so we tell the FXManager that this FX needs to be recycled

            _gameManager.fxManager.KillFX(this);
        }
    }
}