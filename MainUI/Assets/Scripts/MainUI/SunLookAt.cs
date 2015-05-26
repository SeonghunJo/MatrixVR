using UnityEngine;
using System.Collections;

public class SunLookAt : MonoBehaviour {
	public GameObject Earth;
	// Use this for initialization
	void Start () {
	    if(Earth == null)
            Debug.LogWarning("SunLookAt.cs : No Earth Attached");
	}
	
	// Update is called once per frame
	void Update () {
        if(Earth != null)
            transform.rotation.SetLookRotation(Earth.transform.position);
	}
}
