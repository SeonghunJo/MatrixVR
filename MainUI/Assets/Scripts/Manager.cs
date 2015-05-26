using UnityEngine;
using System.Collections;

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
			}
			return _instance;
		}
	}
	// FOR STREETVIEWER.SCENE VARIABLES AND FUNCTIONS
	public string panoramaID;
	public string[] nextIDs;
	public string[] nextDegrees;
	
	public int processCount;
	
	// FOR MainUI.scene VARIABLES AND FUNCTIONS
	
	public string thumbnailID;
	public string thumbnailText;
	public Texture2D thumbnailImg;
	
	// FOR CAMERA RIG
	public Vector3 CameraRotation;
	public Quaternion CameraOrientation;
	
}