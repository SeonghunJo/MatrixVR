using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

	void start()
	{
		//this.renderer.enabled = false;

	}
	public void Clicked()
	{
		Application.LoadLevel ("SceneEarth");
		//this.renderer.material.color.a = 1f;
	
	}
	public void Pointed()
	{
		//this.renderer.enabled = false;

		Color temp = this.renderer.material.color;
		temp.a = 1f;
		this.renderer.material.color = temp;


	}
	public void PointedOut()
	{
		//this.renderer.enabled = false;

		Color temp = this.renderer.material.color;
		temp.a = 0.3f;
		this.renderer.material.color = temp;
	}
}
