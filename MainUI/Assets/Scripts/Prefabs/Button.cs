using UnityEngine;
using System.Collections;

public class Button : MonoBehaviour {

	private string panoramaID;

	public void SetDegree(float degree)
	{
        // Mesh Heart
        /*
		transform.Rotate(new Vector3(280.0f, 180.0f, degree));
		transform.Translate(new Vector3(0.0f, 0.0f, -2.5f));
		transform.Translate(new Vector3(0.0f, -6.5f, 0.0f));
        */
        transform.Rotate(Vector3.right, 90.0f);
        transform.Rotate(Vector3.forward, degree);
        transform.Translate(0, 4, 3, Space.Self);
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
        if(Input.GetKeyDown(KeyCode.B))
        {
            Pointed();
        }
        
        if(Input.GetKeyUp(KeyCode.C))
        {
            PointedOut();
        }
	}

    public void Touch()
    {
        print("Touch");
        
        Manager.Instance.panoramaID = panoramaID;
        GameObject streetViewManager = GameObject.Find("StreetViewManager");
        streetViewManager.GetComponent<StreetViewRenderer>().StartRenderStreetView();
    }

	public void Pointed()  
	{
        Color color = this.renderer.material.color;
        color.a = 1.0f;
        this.renderer.material.color = color;
	}
	public void PointedOut()
	{
        Color color = this.renderer.material.color;
        color.a = 0.3f;
        this.renderer.material.color = color;
	}

	void OnMouseDown()
	{
        Touch();
	}
}
