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
    
	public int processCount;
	
	// FOR MainUI.SCENE VARIABLES AND FUNCTIONS
	
	public string thumbnailID;
	public string thumbnailText;
	public Texture2D thumbnailImg;
    public string wikiText;
	
	// FOR CAMERA RIG
	public Vector3 CameraRotation;
	public Quaternion CameraOrientation;
	
}