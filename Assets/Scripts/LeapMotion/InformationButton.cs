using UnityEngine;
using System.Collections;

public class InformationButton : MonoBehaviour {

	public GameObject InformationCanvas;

	void start()
	{
	}

	public void Clicked()
	{
		InformationCanvas.SetActive (true);
	}

	public void Pointed()
	{
		InformationCanvas.SetActive (true);
	}
	public void PointedOut()
	{

	}
}
