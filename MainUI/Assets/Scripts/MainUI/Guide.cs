using UnityEngine;
using System.Collections;

public class Guide : MonoBehaviour {
	float timeCounter =0;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		timeCounter = Time.time;
		
		if (timeCounter > 15) { 
			gameObject.transform.localScale -= new Vector3(Time.deltaTime*35F,Time.deltaTime*30F,0);
			if(gameObject.transform.localScale.x<=1F |gameObject.transform.localScale.y<=1F)
				Destroy(gameObject);
		}
	}
}
