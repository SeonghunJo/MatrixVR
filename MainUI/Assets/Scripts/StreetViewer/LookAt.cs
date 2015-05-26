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

        if (Input.GetKey(KeyCode.U))
        {
            if (270 < transform.rotation.eulerAngles.x || transform.rotation.eulerAngles.x + 10.0f < 90)
                transform.RotateAround(target.collider.bounds.center, new Vector3(1.0f, 0, 0.0f), 5.0f);
        }
        if (Input.GetKey(KeyCode.J))
        {
            if (270 < transform.rotation.eulerAngles.x - 10.0f || transform.rotation.eulerAngles.x < 90)
                transform.RotateAround(target.collider.bounds.center, new Vector3(-1.0f, 0, 0.0f), 5.0f);
        }
		
		float angle = Manager.Instance.CameraRotation.x;
		
		if(30 < angle && angle < 90)
		{
			//transform.position = new Vector3(transform.position.x + 1f, transform.position.y + 1.0f, transform.position.z);
			transform.RotateAround(target.transform.position, Vector3.left, 1f);
		}
		else if(270 < angle && angle < 330)
		{
			//transform.position = new Vector3(transform.position.x - 1f, transform.position.y - 1.0f, transform.position.z);
			transform.RotateAround(target.transform.position, Vector3.right, 1f);
		}
		else
		{
			//transform.position = new Vector3(transform.position.x, transform.position.y , transform.position.z);
		}
	}
}
