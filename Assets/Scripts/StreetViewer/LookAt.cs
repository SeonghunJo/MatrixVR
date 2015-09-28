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

        if (Input.GetKey(KeyCode.UpArrow))
        {
            if (270 < transform.rotation.eulerAngles.x || transform.rotation.eulerAngles.x + 10.0f < 90)
                transform.RotateAround(target.collider.bounds.center, new Vector3(1.0f, 0, 0.0f), 1.0f);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            if (270 < transform.rotation.eulerAngles.x - 10.0f || transform.rotation.eulerAngles.x < 90)
                transform.RotateAround(target.collider.bounds.center, new Vector3(-1.0f, 0, 0.0f), 1.0f);
        }
		
		float angle = Manager.Instance.CameraRotation.x;
		
		if(25 < angle && angle < 90)
		{
            if (270 < transform.rotation.eulerAngles.x || transform.rotation.eulerAngles.x + 10.0f < 90)
                transform.RotateAround(target.collider.bounds.center, new Vector3(1.0f, 0, 0.0f), 0.8f);
		}
		else if(270 < angle && angle < 335)
		{
            if (270 < transform.rotation.eulerAngles.x - 10.0f || transform.rotation.eulerAngles.x < 90)
                transform.RotateAround(target.collider.bounds.center, new Vector3(-1.0f, 0, 0.0f), 0.8f);
		}
		else
		{
			//transform.position = new Vector3(transform.position.x, transform.position.y , transform.position.z);
		}
	}
}
