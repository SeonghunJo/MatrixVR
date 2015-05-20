using UnityEngine;
using System.Collections;

public class SceneChangeEffect : MonoBehaviour {
	public GameObject leftCamera;
	public GameObject rightCamera;
	// Use this for initialization
	void Start () {
		//leftCamera = GameObject.Find ("LeftEyeAnchor");
		//rightCamera = GameObject.Find ("RightEyeAnchor");
		//Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 100, 0.1);
		leftCamera.camera.fieldOfView = 50;
		leftCamera.camera.fieldOfView = Mathf.Lerp (0, 75, 10);
		rightCamera.camera.fieldOfView = Mathf.Lerp (0, 75, 10);
	
	}
	void update()
	{
		//leftCamera = GameObject.Find ("LeftEyeAnchor");
		//rightCamera = GameObject.Find ("RightEyeAnchor");
		leftCamera.camera.fieldOfView = 50;
	}

}
