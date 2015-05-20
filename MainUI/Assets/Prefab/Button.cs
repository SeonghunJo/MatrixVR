using UnityEngine;
using System.Collections;

public class Button : MonoBehaviour {

	private string panoramaID;

	public void SetDegree(float degree)
	{
		transform.Rotate(new Vector3(280.0f, 180.0f, degree));
		transform.Translate(new Vector3(0.0f, 0.0f, -2.5f));
		transform.Translate(new Vector3(0.0f, -6.5f, 0.0f));
	}

	public void SetPanoramaID(string id)
	{
		panoramaID = id;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}

    public void Touch()
    {
        print("Touch");
        
        Manager.Instance.panoramaID = panoramaID;
        GameObject streetViewManager = GameObject.Find("StreetViewManager");
        streetViewManager.GetComponent<StreetViewRenderer>().StartRenderStreetView();
    }

	void OnMouseDown()
	{
        Touch();
	}
}
