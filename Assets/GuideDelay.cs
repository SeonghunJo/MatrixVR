using UnityEngine;
using System.Collections;

public class GuideDelay : MonoBehaviour {

	public GameObject guide;

	void Start () {
		Invoke ("DelayTest", 2);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void DelayTest()
	{
		guide.SetActive (true);
	}
}
