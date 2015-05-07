using UnityEngine;
using System.IO;
using System.Collections;

public class StreetViewImageRenderer : MonoBehaviour {

    const string API_KEY = "AIzaSyD8CoRMKjg-6awvAx9JGkZyTj1pGubHwQo";
    string key = API_KEY;

	// Texture Size
	public int width = 512;
	public int height = 512;
	
	// Default Location Value
	public double lat = 46.414382;
	public double lon = 10.013988;
	
    public double heading = 90.0;
	public double pitch = -0.0;
    public int fov = 90;

    public string saveTextureFileName = "";
    // Internal Varibales
	double sideHeading = 0.0;
	double sidePitch = 0.0;
    LocationInfo myLocation;
    WWW www;

	// Use this for initialization
	void Start () {

		lat = 127;
		lon = 123;
		Debug.Log("lat : " + lat + " lon : " + lon);
	}

	//https://www.google.com/maps/views/view/streetview/fernando-de-noronha/mirante-1/-dqeDyeJ_jVbfMxRF6jUJw?gl=us&heading=301&pitch=80&fovy=90
	//http://maps.googleapis.com/maps/api/streetview?size=400x400&location=40.720032,%20-73.988354&fov=90&heading=255&pitch=11&sensor=false
	IEnumerator GoogleStreetViewTexture(double _lat, double _lon, double _heading, double _pitch = 0.0)
	{
        string url = "http://maps.googleapis.com/maps/api/streetview?"
            + "size=" + width + "x" + height
            + "&location=" + _lat + "," + _lon
            + "&heading=" + (_heading + sideHeading) % 360.0
            + "&pitch=" + (_pitch + sidePitch) % 360.0
            + "&fov=" + (fov)
            + "&sensor=false";

		if(key != "")
		{
			url += "&key=" + key;
		}

		WWW www = new WWW(url);
		yield return www;
		if(!string.IsNullOrEmpty(www.error))
			Debug.Log("Panorama " + name + ": " + www.error);
		else
			print ("Panorama " + name + " loaded url " + url);


		renderer.material.mainTexture = www.texture;

        if(saveTextureFileName != "")
        {
            string realSavePath = Application.persistentDataPath + "/" + saveTextureFileName + ".png";
            Debug.Log("Encode Texture : " + realSavePath);

            byte[] png = www.texture.EncodeToPNG();
            File.WriteAllBytes(realSavePath, png);
        }
	}

	// Update is called once per frame
	void Update () {
		//float x = Input.GetAxis("Mouse X");
		//float y = Input.GetAxis("Mouse Y");

		if(Input.GetKeyDown(KeyCode.UpArrow))
		{
			print ("UP");
			sidePitch += 1;

			StartCoroutine(GoogleStreetViewTexture(lat, lon, heading, pitch));
		}
		if(Input.GetKeyDown (KeyCode.DownArrow))
		{
			print ("DOWN");
            sidePitch -= 1;

            StartCoroutine(GoogleStreetViewTexture(lat, lon, heading, pitch));
		}
		if(Input.GetKeyDown(KeyCode.LeftArrow))
		{
			print ("LEFT");
			sideHeading -= 1;

            StartCoroutine(GoogleStreetViewTexture(lat, lon, heading, pitch));
		}
		if(Input.GetKeyDown(KeyCode.RightArrow))
		{
			print ("RIGHT");
            sideHeading += 1;

            StartCoroutine(GoogleStreetViewTexture(lat, lon, heading, pitch));
		}

		//g_heading += x * 5;
		//g_pitch += y * 5;

		//print ("Axis : " + x + ", " + y);

	}
}
