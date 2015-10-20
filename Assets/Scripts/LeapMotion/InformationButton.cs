using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InformationButton : MonoBehaviour {

	public GameObject InformationCanvas;
	public GameObject title;
	public GameObject country;
	public GameObject area;
	public GameObject contents;

	void start()
	{
	}

	public void Clicked()
	{

	}

	public void Pointed()
	{
		//Text temp = GetComponent<Text> ();
		Text temp = title.GetComponent<Text> ();
		temp.text = EarthManager.Instance.title;

		temp = country.GetComponent<Text> ();
		temp.text = EarthManager.Instance.country;

		temp = area.GetComponent<Text> ();
		temp.text = EarthManager.Instance.area;

		temp = contents.GetComponent<Text> ();
		temp.text = EarthManager.Instance.contents;

		InformationCanvas.SetActive (true);
	}
	public void PointedOut()
	{

	}
}
