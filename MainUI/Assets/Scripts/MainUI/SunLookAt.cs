using UnityEngine;
using System.Collections;

public class SunLookAt : MonoBehaviour {
	public GameObject Earth;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.rotation.SetLookRotation(Earth.transform.position);

	}
}
