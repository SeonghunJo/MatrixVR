using UnityEngine;
using System.Collections;

public class RevealNightEarthTexture : MonoBehaviour {

	Transform tfLight;
	// Use this for initialization
	void Start () {
		var goLight = GameObject.Find ("RevealingLight");
		if(goLight)
		{
			tfLight = goLight.transform;
		}
	
	}
	
	// Update is called once per frame
	void Update () {
	
		if(tfLight)
		{
			renderer.material.SetVector("_LightPos", tfLight.position);
			renderer.material.SetVector("_LightDir", tfLight.forward);
		}
	}
}
