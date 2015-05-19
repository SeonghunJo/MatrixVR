using UnityEngine;
using System.Collections;

public class AutoRotate : MonoBehaviour {

	public Vector3 speed = new Vector3(0f, 0f, 0f);

	public bool rollAroundParent = false;
	// if roll around
	Transform parentTransform; 
	// Use this for initialization
	void Start () {
		if(rollAroundParent == true) {
			parentTransform = transform.parent;
		}
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(speed * Time.deltaTime);

		if (rollAroundParent) {
			transform.RotateAround(parentTransform.transform.position, speed, 30*Time.deltaTime); 
		}
	}
}
