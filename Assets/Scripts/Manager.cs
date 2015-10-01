using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.IO; // For File Class

// SINGLETON MANAGER CLASS FOR ALL SCENES AND SCRIPTS
public class Manager {
	
	private static Manager _instance;
	public static Manager Instance
	{
		get
		{
			if(_instance == null)
			{
				_instance = new Manager();
			}
			return _instance;
		}
	}

	// FOR STREETVIEWER.SCENE VARIABLES AND FUNCTIONS
	public string panoramaID;
    public bool important;

	public string[] nextIDs;
	public string[] nextDegrees;
    
    public Stack<string> panoramaStack;
    public bool enableAutoGathering = false; // if true, auto gathering
    
	private int progress;

    // Public Method
    public void IncreaseProgress()
    {
        progress++;
    }

    public void SetProgress(int val)
    {
        progress = val;
    }

    public int GetProgress()
    {
        return progress;
    }
}