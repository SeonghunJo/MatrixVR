using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.IO; // For File Class

// SINGLETON MANAGER CLASS FOR ALL SCENES AND SCRIPT 

public class Manager {
	
	private static Manager _instance;
	public static Manager Instance
	{
		get
		{
			if(_instance == null)
			{
				_instance = new Manager();
                cacheFolderPath = Application.persistentDataPath;
			}
			return _instance;
		}
	}

    public bool FindCachedImageFromID(string id)
    {
        if( File.Exists(cacheFolderPath + "/" + id + "_front.png")
            && File.Exists(cacheFolderPath + "/" + id + "_back.png")
            && File.Exists(cacheFolderPath + "/" + id + "_left.png")
            && File.Exists(cacheFolderPath + "/" + id + "_right.png")
            && File.Exists(cacheFolderPath + "/" + id + "_up.png")
            && File.Exists(cacheFolderPath + "/" + id + "_down.png"))
        {
            return true;
        }
        return false;
    }
	// FOR STREETVIEWER.SCENE VARIABLES AND FUNCTIONS
	public string panoramaID;
    public bool important;

	public string[] nextIDs;
	public string[] nextDegrees;
    
    public Stack<string> panoramaStack;
    public bool enableAutoGathering = true;
    private static string cacheFolderPath;

	public int processCount;
	
	// FOR MainUI.scene VARIABLES AND FUNCTIONS
	
	public string thumbnailID;
	public string thumbnailText;
	public Texture2D thumbnailImg;
    public string wikiText;
	
	// FOR CAMERA RIG
	public Vector3 CameraRotation;
	public Quaternion CameraOrientation;
	
}