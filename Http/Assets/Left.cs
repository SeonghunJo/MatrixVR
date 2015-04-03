using UnityEngine;
using System.Collections;

public class Left : MonoBehaviour {
	
	string url = "";
	LocationInfo myLocation;
	WWW www;
	
	// Texture Size
	int width = 400;
	int height = 400;
	
	// Default Location Value
	double lat = 46.414382;
	double lon = 10.013988;
	double g_heading = 0;
	double g_pitch = -0.76;
	
	double sideHeading = 0.0;
	double sidePitch = 0.0;
	
	const string API_KEY = "AIzaSyD8CoRMKjg-6awvAx9JGkZyTj1pGubHwQo";
	
	string key = API_KEY;
	
	// Use this for initialization
	void Start () {
		myLocation = new LocationInfo();
		lat = myLocation.latitude;
		lon = myLocation.longitude;
		lat = 127;
		lon = 123;
		url = "http://maps.google.com/maps/api/staticmap?center="+lat+","+lon+"&zoom=14&size=800x600&maptype=hybrid&sensor=true";
		
		StartCoroutine(MyYield(url));
		Debug.Log("lat : " + lat + " lon : " + lon);
	}
	
	IEnumerator MyYield(string url)
	{
		this.www = new WWW(url);
		
		while(!www.isDone)
		{
			yield return new WaitForSeconds(1);
		}
		Debug.Log("SetMaterial");
		renderer.material.mainTexture = www.texture;
	}
	
	//https://www.google.com/maps/views/view/streetview/fernando-de-noronha/mirante-1/-dqeDyeJ_jVbfMxRF6jUJw?gl=us&heading=301&pitch=80&fovy=90
	//http://maps.googleapis.com/maps/api/streetview?size=400x400&location=40.720032,%20-73.988354&fov=90&heading=255&pitch=11&sensor=false
	IEnumerator GoogleStreetViewTexture(double latitude, double longitude, double heading, double pitch = 0.0)
	{
		string url = "http://maps.googleapis.com/maps/api/streetview?"
			+ "size=" + width + "x" + height
				+ "&location=" + latitude + "," + longitude
				+ "&heading=" + (heading + sideHeading) % 360.0 
				+ "&pitch=" + (pitch + sidePitch) % 360.0
				+ "&fov=90.0&sensor=false";
		
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
	}
	
	// Update is called once per frame
	void Update () {
		float x = Input.GetAxis("Mouse X");
		float y = Input.GetAxis("Mouse Y");
		
		if(Input.GetKeyDown(KeyCode.UpArrow))
		{
			print ("UP");
			g_pitch += 1;
			
			StartCoroutine(GoogleStreetViewTexture(lat, lon, g_heading, g_pitch));
		}
		if(Input.GetKeyDown (KeyCode.DownArrow))
		{
			print ("DOWN");
			g_pitch -= 1;
			
			StartCoroutine(GoogleStreetViewTexture(lat, lon, g_heading, g_pitch));
		}
		if(Input.GetKeyDown(KeyCode.LeftArrow))
		{
			print ("LEFT");
			g_heading -= 1;
			
			StartCoroutine(GoogleStreetViewTexture(lat, lon, g_heading, g_pitch));
		}
		if(Input.GetKeyDown(KeyCode.RightArrow))
		{
			print ("RIGHT");
			g_heading += 1;
			
			StartCoroutine(GoogleStreetViewTexture(lat, lon, g_heading, g_pitch));
		}
		
		//g_heading += x * 5;
		//g_pitch += y * 5;
		
		//print ("Axis : " + x + ", " + y);
		
	}
}
