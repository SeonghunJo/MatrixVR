using UnityEngine;
using System.Collections;

/// <summary>
/// This class controls some GUI elements
/// </summary>
public class ROTD_GUIManager : MonoBehaviour 
{
    /// <summary>
    /// Reference to the game manager
    /// </summary>
    public ROTD_GameManager gameManager;
	
	public Camera guiCamera;
	
    /// <summary>
    /// Reference to the health bar of the chef
    /// </summary>
    public ROTD_ProgressBar chefHealthBar;

    /// <summary>
    /// Reference to the total score
    /// </summary>
    public ROTD_Score totalScore;

    /// <summary>
    /// Reference to the total kills
    /// </summary>
    public ROTD_Score pizzaCountScore;

    /// <summary>
    /// Reference to a sprite that shows how to play
    /// </summary>
    public SmoothMoves.BoneAnimation instructionsAnimation;

    /// <summary>
    /// Reference to the game over animation
    /// </summary>
    public SmoothMoves.BoneAnimation gameOverAnimation;

    /// <summary>
    /// Reference to the new weapon available animation
    /// </summary>
    public SmoothMoves.BoneAnimation newWeaponAvailableAnimation;

    /// <summary>
    /// Whether or not the instructions have gone away
    /// </summary>
    public bool InstructionsDismissed { get; set; }

    /// <summary>
    /// Called once before other scripts
    /// </summary>
    void Awake()
    {
        // initialize that the instructions are visible
        InstructionsDismissed = false;

        // register the user trigger for the game animation
        gameOverAnimation.RegisterUserTriggerDelegate(GameOver_UserTrigger);

        // register the user trigger for the instructions animation
        instructionsAnimation.RegisterUserTriggerDelegate(Instructions_UserTrigger);

        // register the user trigger for the new weapon available animation
        newWeaponAvailableAnimation.RegisterUserTriggerDelegate(NewWeaponAvailable_UserTrigger);

        // hide the game over text
        ShowGameOver(false);

        // show the instructions sprite
        ShowInstructions(true);

        // move the instructions and game over to the screen
        // (we have this offset in the editor so that we can see what we are doing)
        gameOverAnimation.gameObject.transform.localPosition = new Vector3(0, 0, gameOverAnimation.gameObject.transform.localPosition.z);
        instructionsAnimation.gameObject.transform.localPosition = new Vector3(0, 0, instructionsAnimation.gameObject.transform.localPosition.z);
    }

    /// <summary>
    /// User Trigger delegate that is fired from the game over animation
    /// </summary>
    /// <param name="utEvent">Event sent from the animation</param>
    public void GameOver_UserTrigger(SmoothMoves.UserTriggerEvent utEvent)
    {
        // done event was sent
        if (utEvent.tag == "Done")
        {
            gameManager.State = ROTD_GameManager.STATE.WaitingForInput;
        }
    }

    /// <summary>
    /// User Trigger delegate that is fired from the instructions animation
    /// </summary>
    /// <param name="utEvent">Event sent from the animation</param>
    public void Instructions_UserTrigger(SmoothMoves.UserTriggerEvent utEvent)
    {
        if (utEvent.tag == "Done")
        {
#if UNITY_3_5
            instructionsAnimation.gameObject.active = false;
#else
            instructionsAnimation.gameObject.SetActive(false);
#endif
            InstructionsDismissed = true;
        }
    }

    /// <summary>
    /// User Trigger delegate that is fired from the new weapon available animation
    /// </summary>
    /// <param name="utEvent">Event sent from the animation</param>
    public void NewWeaponAvailable_UserTrigger(SmoothMoves.UserTriggerEvent utEvent)
    {
        if (utEvent.tag == "Done")
        {
#if UNITY_3_5
            newWeaponAvailableAnimation.gameObject.active = false;
#else
            newWeaponAvailableAnimation.gameObject.SetActive(false);
#endif
        }
    }

    /// <summary>
    /// Shows or hides the instructions
    /// </summary>
    /// <param name="show"></param>
    public void ShowInstructions(bool show)
    {
        if (show)
        {
#if UNITY_3_5
            instructionsAnimation.gameObject.active = true;
#else
            instructionsAnimation.gameObject.SetActive(true);
#endif
        }
        else
        {
#if UNITY_3_5
            if (instructionsAnimation.gameObject.active)
                instructionsAnimation.Play("Dismiss");
#else
            if (instructionsAnimation.gameObject.activeSelf)
                instructionsAnimation.Play("Dismiss");
#endif
        }
    }

    /// <summary>
    /// Shows or hides the game over text
    /// </summary>
    public void ShowGameOver(bool show)
    {
        if (show)
        {
#if UNITY_3_5
            gameOverAnimation.gameObject.SetActiveRecursively(true);
#else
            gameOverAnimation.gameObject.SetActive(true);
#endif
            gameOverAnimation.Play("Game_Over");
        }
        else
        {
#if UNITY_3_5
            gameOverAnimation.gameObject.SetActiveRecursively(false);
#else
            gameOverAnimation.gameObject.SetActive(false);
#endif
        }
    }

    /// <summary>
    /// Animates when a new weapon is available
    /// </summary>
    public void ShowNewWeaponAvailable(bool show)
    {
        if (show)
        {
#if UNITY_3_5
            newWeaponAvailableAnimation.gameObject.active = true;
#else
            newWeaponAvailableAnimation.gameObject.SetActive(true);
#endif
            newWeaponAvailableAnimation.Play("Show");
        }
        else
        {
#if UNITY_3_5
            newWeaponAvailableAnimation.gameObject.active = false;
#else
            newWeaponAvailableAnimation.gameObject.SetActive(false);
#endif
        }
    }

    /// <summary>
    /// Resets the GUI back to a starting state
    /// </summary>
    public void ResetToStart()
    {
        ShowGameOver(false);
        ShowInstructions(false);
        ShowNewWeaponAvailable(false);

        totalScore.Value = "0";
        pizzaCountScore.Value = "0";

        chefHealthBar.ResetToStart();
    }
}
