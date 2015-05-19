using UnityEngine;
using System.Collections;
using LitJson;

public class StreetviewPoint : MonoBehaviour
{
    //해당 point의 위도, 경도, 파노라마 ID를 저장
    public float Lat;
    public float Lng;
    public string panoID;

    //Thumbnail image URL & Streetview name URL
	public string thumbnailURL = "http://maps.google.com/cbk?output=thumbnail&panoid=";
	public string metaURL = "http://maps.google.com/cbk?output=json&panoid=";

    //Thumbnail image, name, ID 
	public Texture2D myThumbnailImg;
	public string myThumbnailText;
    OVRThumbnailUI thumbnailUI;

	// Use this for initialization
    IEnumerator Start() {		
		Debug.Log ("start : " + panoID);
        thumbnailUI = GameObject.Find("LeapOVRCameraRig").GetComponent<OVRThumbnailUI>();

		WWW www = new WWW(thumbnailURL);
		yield return www;

        myThumbnailImg = www.texture;
		Manager.Instance.thumbnailImg = myThumbnailImg;

        StartCoroutine(GetLocationText(metaURL + panoID));
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.UpArrow)) {
            Application.LoadLevel("StreetViewer");
        }
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

	public void Pointed ()
	{
        Manager.Instance.thumbnailText = myThumbnailText;
        Manager.Instance.thumbnailImg = myThumbnailImg;
        Manager.Instance.panoramaID = panoID;

        thumbnailUI.ShowScreen();
	}
	
    void OnMouseEnter()
	{
		Debug.Log("mouse enter : " + panoID);

        Pointed();
	}

	public void PointedOut()
	{
        Debug.Log("destroy : " + panoID);

        thumbnailUI.HideScreen();
	}

    void OnMouseExit()
    {
        PointedOut();
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

    IEnumerator GetLocationText(string url)
    {
        WWW www = new WWW(url);
        yield return www;

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
    }
}
