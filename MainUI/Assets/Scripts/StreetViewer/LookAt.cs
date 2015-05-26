using UnityEngine;
using System.Collections;

public class LookAt : MonoBehaviour {
	
	public GameObject target;
	public Vector3 beforeRotation;
	
	
	// Use this for initialization
	void Start () {
		beforeRotation = Manager.Instance.CameraRotation;
	}
	
	// Update is called once per frame
	void Update () {
		if(target != null)
		{
			transform.LookAt(target.transform.position);
		}
		
		if(Input.GetKey(KeyCode.U))
		{
			transform.position = new Vector3(transform.position.x - 1f, transform.position.y + 1.0f, transform.position.z);
		}
		if(Input.GetKey(KeyCode.J))
		{
			transform.position = new Vector3(transform.position.x + 1f, transform.position.y - 1.0f, transform.position.z);
		}
		
		float angle = Manager.Instance.CameraRotation.x;
		
		if(30 < angle && angle < 90)
		{
			transform.position = new Vector3(transform.position.x + 1f, transform.position.y + 1.0f, transform.position.z);
		}
		else if(270 < angle && angle < 330)
		{
			transform.position = new Vector3(transform.position.x - 1f, transform.position.y - 1.0f, transform.position.z);
		}
		else
		{
			//transform.position = new Vector3(transform.position.x, transform.position.y , transform.position.z);
		}
	}
}