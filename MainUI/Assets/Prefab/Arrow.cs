using UnityEngine;
using System.Collections;

public class Arrow : MonoBehaviour {

	public void SetDegree(float degree)
	{
		transform.Rotate(new Vector3(280.0f, 180.0f, degree));
		transform.Translate(new Vector3(0.0f, 0.0f, -2.5f));
		transform.Translate(new Vector3(0.0f, -6.5f, 0.0f));
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
