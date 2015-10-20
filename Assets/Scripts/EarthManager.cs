using UnityEngine;
using System.Collections;

public class EarthManager {
	private static EarthManager _instance;
	public static EarthManager Instance
	{
		get
		{
			if(_instance == null)
			{
				_instance = new EarthManager();
			}
			return _instance;
		}
	}
	public int startGuide;

    // FOR MainUI.SCENE VARIABLES AND FUNCTIONS
    public string thumbnailID;
    public string thumbnailText;
    public Texture2D thumbnailImg;
    public string wikiText;

	public string title;
	public string country;
	public string area;
	public string contents;

    // FOR CAMERA RIG
    public Vector3 CameraRotation;
    public Quaternion CameraOrientation;

}
