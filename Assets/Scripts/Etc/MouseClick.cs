using UnityEngine;
using System.Collections;

public class MouseClick : MonoBehaviour {

	public Camera leftCamera;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
						
		float distance;
		
		Vector3 pos;
		
		Ray ray = leftCamera.ScreenPointToRay(Input.mousePosition);
		
				
		RaycastHit hit;
		
		
		
		if(Physics.Raycast(ray,out hit))
			
		{
			
			Debug.Log(hit.collider.name);
			
		} 	
	}
}
