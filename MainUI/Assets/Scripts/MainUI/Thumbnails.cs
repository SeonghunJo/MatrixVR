using UnityEngine;
using System.Collections;

public class Thumbnails : MonoBehaviour {

    public string URL = "http://maps.google.com/cbk?output=thumbnail&panoid=";
    
    // Use this for initialization
    //void Start () {
	
    //}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetPosition(string id)
    {
        URL = URL + id;
    }

    IEnumerator Start()
    {
        WWW www = new WWW(URL);
        yield return www;

        renderer.material.mainTexture = www.texture;

        transform.Translate(new Vector3(2.0f,-2.0f, -2.5f));

        Debug.Log("url = " + URL);
    }
}
