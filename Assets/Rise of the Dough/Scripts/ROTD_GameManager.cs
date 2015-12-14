using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// This is a singleton class that ties all the game elements together. It simplifies the 
/// scene design so that you don't have to have a spaghetti network of links between objects.
/// All objects can point here to get access to any other object.
/// </summary>
public class ROTD_GameManager : MonoBehaviour
{
    /// <summary>
    /// Internal state of the game
    /// </summary>
    private STATE _state;

    /// <summary>
    /// Internal list of difficulty levels sorted by total kill threshold
    /// (in case the user entered them out of order)
    /// </summary>
    private List<ROTD_DifficultyLevel> _internalDifficultyLevels;

    /// <summary>
    /// possible states of the game
    /// </summary>
    public enum STATE
    {
        Initializing,
        WaitingForInput,
        Playing,
        GameOver
    }
	
	public Camera mainCamera;

    /// <summary>
    /// Reference to the camera controller
    /// </summary>
    public ROTD_CameraController cameraController;

    /// <summary>
    /// Reference to the chef
    /// </summary>
    public ROTD_Chef chef;

    /// <summary>
    /// Reference to the FX Manager
    /// </summary>
    public ROTD_FXManager fxManager;

    /// <summary>
    /// Reference to the GUI manager
    /// </summary>
    public ROTD_GUIManager guiManager;

    /// <summary>
    /// Reference to the Input Manager
    /// </summary>
    public ROTD_InputManager inputManager;

    /// <summary>
    /// Reference to the Music Manager
    /// </summary>
    public ROTD_MusicManager musicManager;

    /// <summary>
    /// Reference to the Pasta Manager
    /// </summary>
    public ROTD_PastaManager pastaManager;

    /// <summary>
    /// Reference to the Room
    /// </summary>
    public ROTD_Room room;

    /// <summary>
    /// Reference to the knife that can be picked up
    /// </summary>
    public ROTD_Weapon weapon;

    /// <summary>
    /// Reference to the Sound FX Manager
    /// </summary>
    public ROTD_SoundFXManager soundFXManager;

    /// <summary>
    /// These are the total kill amounts that increase
    /// the difficulty by releasing another pasta
    /// </summary>
    public ROTD_DifficultyLevel[] difficultyLevels;

    /// <summary>
    /// These are the total kill amounts that 
    /// improve the upgrade weapon
    /// </summary>
    public ROTD_WeaponUpgrade[] weaponUpgrades;
	
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
                case STATE.Initializing:
                case STATE.WaitingForInput:

                    // no need to change anything with these states

                    break;

                case STATE.Playing:

                    // reset the managers

                    ResetToStart();

                    break;

                case STATE.GameOver:

                    // stop the music
                    musicManager.StopMusic();

                    // set the game over text
                    guiManager.ShowGameOver(true);

                    // play the game over sound effect
                    soundFXManager.Play("game_over");

                    break;
            }
        }
    }

    /// <summary>
    /// Keeps track of how many pastas have been killed.
    /// This is used to increase the difficulty progressively.
    /// </summary>
    public int TotalPastaKills { get; set; }

    /// <summary>
    /// Keeps track if the user is looking to buy Smooth Moves
    /// </summary>
    public bool BuyNow { get; set; }

    /// <summary>
    /// Called before other scripts
    /// </summary>
    void Awake()
    {
		// set the orthographic size of the main and GUI cameras based on the platform
		
#if UNITY_IPHONE
		switch (iPhone.generation)
		{
		case iPhoneGeneration.iPad1Gen:
		case iPhoneGeneration.iPad2Gen:
		case iPhoneGeneration.iPad3Gen:
			mainCamera.orthographicSize = 384.0f;
			guiManager.guiCamera.orthographicSize = 384.0f;
			break;	
			
		default:
			mainCamera.orthographicSize = 320.0f;
			guiManager.guiCamera.orthographicSize = 320.0f;
			break;			
		}
#else
		mainCamera.orthographicSize = 320.0f;
		guiManager.guiCamera.orthographicSize = 320.0f;
#endif

        // create an internal list of difficulties based on the user input
        // and sort by the total kill threshold
        _internalDifficultyLevels = new List<ROTD_DifficultyLevel>();
        foreach (ROTD_DifficultyLevel dl in difficultyLevels)
        {
            _internalDifficultyLevels.Add(dl);
        }
        _internalDifficultyLevels.Sort(new ROTD_SortDifficultyLevelDescending());

        // initialize state
        State = STATE.Initializing;

    }

    /// <summary>
    /// Called once at the start of the scene
    /// </summary>
    void Start()
    {
        // initialize the total score to zero
        guiManager.totalScore.Value = "0";

        // initialize the state
        State = STATE.WaitingForInput;
    }

    /// <summary>
    /// Called once per frame
    /// </summary>
    void Update()
    {
        switch (_state)
        {
            case STATE.Initializing:

                // no need to update anything while initializing

                break;

            case STATE.WaitingForInput:

                // update the frame manager
                inputManager.FrameUpdate();

                break;

            case STATE.Playing:

                // update the managers

                inputManager.FrameUpdate();
                chef.FrameUpdate();
                room.FrameUpdate();

                if (guiManager.InstructionsDismissed)
                    pastaManager.FrameUpdate();

                break;

            case STATE.GameOver:

                // keep updating the pasta on the fade out

                pastaManager.FrameUpdate();

                break;
        }
    }

    /// <summary>
    /// Shows a score FX animation at the location of the the dead pasta and 
    /// updates the GUI total score
    /// </summary>
    /// <param name="position"></param>
    public void AddScore(Vector3 position)
    {
        // increase the number of total kills
        TotalPastaKills++;

        // get the current score value from the Pasta Manager (this score is incremented each kill)
        int newScore = pastaManager.CurrentPastaScore;

        // tell the FX manager to activate a score animation at the location of the kill
        fxManager.Activate(ROTD_FX.FX_TYPE.Score, position + new Vector3(0, pastaManager.scoreYOffset, 0), 0, newScore.ToString());

        // tell the GUI manager to update the total score
        guiManager.totalScore.SetScoreAndBounce(guiManager.totalScore.IntValue + newScore);

        // tell the GUI manager to update the pizza count
        guiManager.pizzaCountScore.SetScoreAndBounce(TotalPastaKills);

        // check for a weapon upgrade
        SetWeaponUpgrade();
    }

    /// <summary>
    /// Resets the states of the managers to the start
    /// </summary>
    private void ResetToStart()
    {
        TotalPastaKills = 0;

        inputManager.TurnOffBuyNowTouchPad();

        chef.ResetToStart();
        pastaManager.ResetToStart();
        fxManager.ResetToStart();
        guiManager.ResetToStart();
        weapon.ResetToStart();
        musicManager.ResetToStart();

        SetWeaponUpgrade();
    }

    /// <summary>
    /// Gets the number of pastas for each type that need to be active
    /// based on the total number of kills
    /// </summary>
    /// <returns>The number of active pastas</returns>
    public int GetCurrentPastaTypeCount(ROTD_Pasta.TYPE pastaType)
    {
        // go through the difficulty levels, starting with the highest first
        foreach (ROTD_DifficultyLevel dl in _internalDifficultyLevels)
        {
            // if the total kills is higher than this difficulty level threshold
            if (TotalPastaKills >= dl.totalKillThreshold)
            {
                // look for the correct pasta type and return the count
                switch (pastaType)
                {
                    case ROTD_Pasta.TYPE.Pizza:
                        return dl.pizzaCount;
                }
            }
        }

        // no difficulty level reached or no pasta type found, just return zero
        return 0;
    }

    /// <summary>
    /// Opens a browser to buy Smooth Moves plugin from the Unity Asset Store
    /// </summary>
    public void ShowAssetStoreLink()
    {
        BuyNow = true;
        Application.OpenURL("http://u3d.as/content/echo17/smooth-moves/2Fn");
    }

    /// <summary>
    /// Sets the proper weapon upgrade based on the total number of kills
    /// </summary>
    private void SetWeaponUpgrade()
    {
        // go through the weapon upgrades, starting with the highest first
        foreach (ROTD_WeaponUpgrade wu in weaponUpgrades)
        {
            // if the total kills is equal to this weapon upgrade threshold
            if (TotalPastaKills == wu.totalKillThreshold)
            {
                // set the weapon type and turn it on
                weapon.WeaponType = wu.weaponType;
                weapon.Toggle(true);

                // play a sound to let the user know a new weapon upgrade is available
                soundFXManager.Play("upgrade_available");

                // show that a new weapon upgrade is available
                guiManager.ShowNewWeaponAvailable(true);

                break;
            }
        }
    }
}
