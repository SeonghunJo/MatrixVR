using UnityEngine;
using System.Collections;
using LitJson;

public class StreetviewPoint : MonoBehaviour
{
    public float Lat;
    public float Lng;
    public string panoID;

	public string thumbnailURL = "http://maps.google.com/cbk?output=thumbnail&panoid=";
	public string metaURL = "http://maps.google.com/cbk?output=json&panoid=";

	public Texture2D myThumbnailImg;
	public string myThumbnailText;

	// Use this for initialization
    IEnumerator Start() {		
		Debug.Log ("start : " + panoID);

		WWW www = new WWW(thumbnailURL);
		yield return www;

		myThumbnailImg = www.texture;

		Manager.Instance.thumbnailImg = myThumbnailImg;

		WWWHelper helper = WWWHelper.Instance;
		helper.OnHttpRequest += OnHttpRequest;
		helper.get(100, metaURL + panoID);

    }

    // Update is called once per frame
    void Update()
    {

    }
    void FadeOutEnd()
    {
        //panoID에 따라 Scene 전환(street view)
        Application.LoadLevel("StreetViewer");

    }

	public void Clicked()
	{
		CameraFade.setFadeOutEndEvent(FadeOutEnd);
		CameraFade.FadeOutMain();
	}

    void OnMouseDown()
    {
        CameraFade.setFadeOutEndEvent(FadeOutEnd);
        CameraFade.FadeOutMain(); 
    }

	public void Pointed ()
	{

	}
	void OnMouseEnter()
	{
		Debug.Log("mouse enter : " + panoID);
		/*
		Manager.Instance.thumbnailText = myThumbnailText;
		Manager.Instance.thumbnailImg = myThumbnailImg;

		Debug.Log ("myThumbnailText : " + myThumbnailText);
		Debug.Log ("myTumbnailImg : " + myThumbnailImg);
*/
		OVRThumbnailUI thumbnailImg = GameObject.Find ("LeapOVRCameraRig").GetComponent<OVRThumbnailUI>();
		thumbnailImg.ShowScreen();
	}

	public void PointedOut()
	{

	}

    void OnMouseExit()
    {
        Debug.Log("destroy : " + panoID);

		OVRThumbnailUI thumbnailImg = GameObject.Find ("LeapOVRCameraRig").GetComponent<OVRThumbnailUI>();
		thumbnailImg.HideScreen();
    }

    public void SetPosition()
    {
        Lat = CreatePoint._rotation.x;
        Lng = -(CreatePoint._rotation.y);
        panoID = CreatePoint.panoID;

		thumbnailURL = thumbnailURL + panoID;

		Debug.Log(Lat + " " + Lng + " " + panoID );

        transform.Rotate(CreatePoint._rotation);
        transform.Translate(CreatePoint._translate);

    }

	// For WWWHelper Class
	void OnHttpRequest(int id, WWW www)
	{
		if (www.error != null)
		{
			Debug.Log("[Error] " + www.error);
			return;
		}
		
		string description;
		string country;
		string region;
		
		string locationText = "";
		JsonData json = JsonMapper.ToObject(www.text);
		JsonData data = json["Data"];
		
		JsonData location = json["Location"];
		
		if (location.Keys.Contains("description"))
		{
			description = location["description"].ToString();
			locationText += description;
		}
		if (location.Keys.Contains("country"))
		{
			country = location["country"].ToString();
			locationText += ", " + country;
		}
		if (location.Keys.Contains("region"))
		{
			region = location["region"].ToString();
			locationText += ", " + region;
		}
		myThumbnailText = locationText;

		Manager.Instance.thumbnailText = myThumbnailText;
	}
}
