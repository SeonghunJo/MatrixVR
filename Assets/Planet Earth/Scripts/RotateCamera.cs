using UnityEngine;
using System.Collections;

public class RotateCamera : MonoBehaviour {

	private float mouseXAmount, mouseYAmount;
	public Transform cam;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
		if(Input.GetKey(KeyCode.A)){
			transform.Rotate(0,0,0.1f);
			
		}
		if(Input.GetKey(KeyCode.E)){
			transform.Rotate(0,0,-0.1f);
			
		}
		if(Input.GetKey(KeyCode.LeftArrow)){
			transform.Rotate(0,0.05f,0);
			
		}
		if(Input.GetKey(KeyCode.RightArrow)){
			transform.Rotate(0,-0.05f,0);
			
		}
		
		if(Input.GetKey(KeyCode.UpArrow)){
			transform.Rotate(0.05f,0,0);
			
		}
		if(Input.GetKey(KeyCode.DownArrow)){
			transform.Rotate(-0.05f,0,0);

		}
		
		if(Input.GetKey(KeyCode.Z)){
			cam.Rotate(-0.05f,0,0);
			
		}
		if(Input.GetKey(KeyCode.S)){
			cam.Rotate(0.05f,0,0);
			
		}
		if(Input.GetKey(KeyCode.Q)){
			cam.Rotate(0,-0.05f,0);
			
		}
		if(Input.GetKey(KeyCode.D)){
			cam.Rotate(0,0.05f,0);
			
		}
	}
}
