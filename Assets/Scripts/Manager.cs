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
	private string placeID;
    private bool bHotPlace;

	public string[] nextIDs;
	public string[] nextDegrees;

    private int progress; 
    private string loadingText;

    public bool enableAutoImageCache = false; // if true, auto image caching


    public void SetLoadingText(string text)
    {
        loadingText = text;
    }

    public string GetLoadingText()
    {
        return loadingText;
    }

    public void SetHotPlace(bool isHotPlace)
    {
        bHotPlace = isHotPlace;
    }

    public bool IsHotPlace()
    {
        return bHotPlace;
    }
    
    public void SetPlaceID(string panoid)
    {
        placeID = panoid;
    }

    public string GetPlaceID()
    {
        return placeID;
    }

    public void IncreaseProgress(int val = 1)
    {
        progress += val;
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