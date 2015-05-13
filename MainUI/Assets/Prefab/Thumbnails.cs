using UnityEngine;
using System.Collections;
using LitJson;

public class Thumbnails : MonoBehaviour {

    public GameObject image;
    public GameObject textobject;
    public string text;

    public string thumbnailURL = "http://maps.google.com/cbk?output=thumbnail&panoid=";
    public string metaURL = "http://maps.google.com/cbk?output=json&panoid=";

    // Use this for initialization
    //void Start () {
	
    //}
	
	// Update is called once per frame
	void Update () {
	
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
        text = locationText;

        Debug.Log(text);
    }

    public void SetPosition(string id)
    {
        thumbnailURL = thumbnailURL + id;

        WWWHelper helper = WWWHelper.Instance;
        helper.OnHttpRequest += OnHttpRequest;
        helper.get(100, metaURL + id);
    }

    IEnumerator Start()
    {
        thumbnailURL = "http://maps.google.com/cbk?output=thumbnail&panoid=TXymVghDgJk2ViaA69pNaQ";

        WWW www = new WWW(thumbnailURL);
        yield return www;

        image.renderer.material.mainTexture = www.texture;
        textobject.GetComponent<TextMesh>().text = text;

        image.transform.Translate(new Vector3(0.0f,-1.2f, -3.0f));
        textobject.transform.Translate(new Vector3(0.0f, -2.0f, -3.0f));

        Debug.Log("url = " + thumbnailURL);
    }
}
