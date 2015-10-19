using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// This class manages some global sound effects
/// </summary>
public class ROTD_SoundFXManager : MonoBehaviour 
{
	/// <summary>
	/// Internal list of sounds, referenced by their name
	/// </summary>
	private Dictionary<string, AudioSource> _soundDictionary;
	
    /// <summary>
    /// Called once before other scripts
    /// </summary>
	void Awake()
	{
		// cache our audio sources into a dictionary so that we can quickly play 
		// a sound by using its name
		_soundDictionary = new Dictionary<string, AudioSource>();
		
		Component [] components = gameObject.GetComponentsInChildren(typeof(AudioSource));
		AudioSource sound;
		
        // gather the sounds into the dictionary
		foreach (Component component in components)
		{
			sound = (AudioSource)component;
			_soundDictionary.Add (sound.clip.name, sound);
		}
	}
	
    /// <summary>
    /// Plays a sound by its name
    /// </summary>
    /// <param name="soundName">Name of the Sound FX</param>
	public void Play(string soundName)
	{
        // if the dictionary contains the sound FX name
		if (_soundDictionary.ContainsKey(soundName))
		{
            // play the sound
			_soundDictionary[soundName].Play();
		}
	}

    /// <summary>
    /// Plays a sound by its name with a randomized pitch
    /// </summary>
    /// <param name="soundName">Name of the Sound FX</param>
    /// <param name="minPitch">The minimum level of pitch</param>
    /// <param name="maxPitch">The maximum level of pitch</param>
    public void Play(string soundName, float minPitch, float maxPitch)
    {
        // if the dictionary contains the sound FX name
        if (_soundDictionary.ContainsKey(soundName))
        {
            // play the sound
            _soundDictionary[soundName].pitch = UnityEngine.Random.Range(minPitch, maxPitch);
            _soundDictionary[soundName].Play();
        }
    }
}
