using UnityEngine;
using System.Collections;

/// <summary>
/// This class plays the music for the game
/// </summary>
public class ROTD_MusicManager : MonoBehaviour 
{
    /// <summary>
    /// Reference to the game manager
    /// </summary>
    public ROTD_GameManager gameManager;

    /// <summary>
    /// Whether or not to play the music
    /// </summary>
	public bool playMusic;

    /// <summary>
    /// The music player
    /// </summary>
	public AudioSource player;

    /// <summary>
    /// Resets the music back to its initial state
    /// </summary>
    public void ResetToStart()
    {
        if (playMusic)
        {
            player.Play();
        }
    }
	
    /// <summary>
    /// Stops the music
    /// </summary>
    public void StopMusic()
    {
        player.Stop();
    }
}

