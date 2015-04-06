using UnityEngine;
using System.Collections;

public class ButtonClick : MonoBehaviour {


	// Use this for initialization
	void Start () {
        CameraFade2.setFadeOutEndEvent(FadeOutEnd);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 50, 30), "Back"))
            CameraFade2.FadeOutMain();
    }


    void FadeOutEnd()
    {
        Application.LoadLevel(0);
    }
}