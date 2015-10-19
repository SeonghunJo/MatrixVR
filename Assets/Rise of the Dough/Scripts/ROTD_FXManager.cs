using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// This class manages the FX objects that are instantiated in the scene. Having a manager class like this cuts
/// down on the instantiation at runtime and improves memory efficiency and speed. FX objects are recycled after
/// they are "killed".
/// </summary>
public class ROTD_FXManager : MonoBehaviour 
{
    /// <summary>
    /// The internal list of FX objects
    /// </summary>
    private List<ROTD_FX> _fxList = new List<ROTD_FX>();

    /// <summary>
    /// Reference to the game manager
    /// </summary>
    public ROTD_GameManager gameManager;

    /// <summary>
    /// Reference to the prefab for the sauce animation
    /// </summary>
    public GameObject saucePrefab;

    /// <summary>
    /// Number of sauce objects to create
    /// Trying to use more than this number at runtime will be ignored
    /// </summary>
    public int sauceCount;

    /// <summary>
    /// Reference to the prefab for the pizza splat animation
    /// </summary>
    public GameObject pizzaSplatPrefab;

    /// <summary>
    /// Number of pizza splats to create
    /// Trying to use more than this number at runtime will be ignored
    /// </summary>
    public int pizzaSplatCount;

    /// <summary>
    /// Reference to teh prefab for the score animation
    /// </summary>
    public GameObject scorePrefab;

    /// <summary>
    /// Number of scores to create
    /// Trying to use more than this number at runtime will be ignored
    /// </summary>
    public int scoreCount;

    /// <summary>
    /// The Z position for background objects, like splats and sauce.
    /// This prevents these objects to overlap foreground objects
    /// </summary>
    public float backgroundZPosition;

    /// <summary>
    /// Called once at the start of the scene
    /// </summary>
	void Start () 
    {
        GameObject go;
        ROTD_FX fx;

        // instantiate the sauce objects
        for (int i = 0; i < sauceCount; i++)
        {
            // create the object and place it under this manager
            go = (GameObject)Instantiate(saucePrefab, Vector3.zero, Quaternion.identity);
            go.transform.parent = this.transform;

            // grab the FX class component and initialize
            fx = go.GetComponent<ROTD_FX>();
            fx.Initialize(gameManager);

            // add the FX object to the internal list
            _fxList.Add(fx);
        }

        // instantiate the pizza splat objects
        for (int i = 0; i < pizzaSplatCount; i++)
        {
            // create the object and place it under this manager
            go = (GameObject)Instantiate(pizzaSplatPrefab, Vector3.zero, Quaternion.identity);
            go.transform.parent = this.transform;

            // grab the FX class component and initialize
            fx = go.GetComponent<ROTD_FX>();
            fx.Initialize(gameManager);

            // add the FX object to the internal list
            _fxList.Add(fx);
        }

        for (int i = 0; i < scoreCount; i++)
        {
            // create the object and place it under this manager
            go = (GameObject)Instantiate(scorePrefab, Vector3.zero, Quaternion.identity);
            go.transform.parent = this.transform;

            // grab the FX class component and initialize
            fx = go.GetComponent<ROTD_FX>();
            fx.Initialize(gameManager);

            // add the FX object to the internal list
            _fxList.Add(fx);
        }
    }

    /// <summary>
    /// This function tells the FX manager to recycle a non-active (Dead) FX object,
    /// depending on its type
    /// </summary>
    /// <param name="type">The type of the FX object to recycle</param>
    /// <param name="position">The location of the FX object</param>
    /// <param name="direction">The direction the FX object should face (1 = same as the animation, -1 = mirror along the x axis of the animation)</param>
    /// <param name="value">The value to assign to the FX object</param>
    public void Activate(ROTD_FX.FX_TYPE type, Vector3 position, int direction, string value)
    {
        // check each FX object in the internal list
        foreach (ROTD_FX fx in _fxList)
        {
            // if the FX object is dead and is of the type we are looking for then recycle it
            if (fx.State == ROTD_FX.STATE.Dead && fx.fxType == type)
            {
                // reset the FX object, bringing it back to life
                fx.Reset(position, direction, value);
                break;
            }
        }
    }

    /// <summary>
    /// This function is called from the FX object, notifying the manager that
    /// the object needs to be deactivated.
    /// Note that this does not destroy the object, merely turns it "off" for 
    /// reuse later.
    /// </summary>
    /// <param name="fx">The FX object to deactivate</param>
    public void KillFX(ROTD_FX fx)
    {
        // set the state of the FX object to Dead
        fx.State = ROTD_FX.STATE.Dead;
    }

    /// <summary>
    /// Resets the FX back to the starting states
    /// </summary>
    public void ResetToStart()
    {
        foreach (ROTD_FX fx in _fxList)
        {
            KillFX(fx);
        }
    }
}
