using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

	void start()
	{
		this.renderer.enabled = false;
	}
	public void Clicked()
	{
		Application.LoadLevel ("SceneEarth");
	}
	public void Pointed()
	{
		this.renderer.enabled = true;
	}
	public void PointedOut()
	{
		this.renderer.enabled = false;
	}
}
