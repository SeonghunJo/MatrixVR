using UnityEngine;
using System.Collections;

public class Button : MonoBehaviour {

	public string panoramaID;
    public float yawDegree;

	public void SetDegree(float degree)
	{
        // Mesh Heart
        /*
		transform.Rotate(new Vector3(280.0f, 180.0f, degree));
		transform.Translate(new Vector3(0.0f, 0.0f, -2.5f));
		transform.Translate(new Vector3(0.0f, -6.5f, 0.0f));
        */

        yawDegree = degree;

        transform.Rotate(Vector3.right, 90.0f); // 세워진 화살표를 눕히고
        transform.Rotate(Vector3.forward, degree - 90.0f); // 화살표의 전방방향으로 Yaw만큼 회전한 후 90도의 오차만큼 회전한다.
        transform.Translate(0, 6, 3, Space.Self);
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
