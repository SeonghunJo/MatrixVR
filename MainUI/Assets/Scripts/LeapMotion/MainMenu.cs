using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

	//private Vector3 scale;

	void start()
	{
		//scale = this.transform.localScale;
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
		//Vector3 tempscale;
		//tempscale.x = scale.x * 2;
		//tempscale.y = scale.y * 2;
		//tempscale.z = scale.z * 2;

		Color temp = this.renderer.material.color;
		temp.a = 1f;
		this.renderer.material.color = temp;
		this.transform.localScale = new Vector3 (1.2f, 1.2f, 1.2f);


	}
	public void PointedOut()
	{
		//this.renderer.enabled = false;

		Color temp = this.renderer.material.color;
		temp.a = 0.3f;
		this.renderer.material.color = temp;
		//this.transform.localScale = scale;
		this.transform.localScale = new Vector3 (0.8f, 0.8f, 0.8f);
	}
}
