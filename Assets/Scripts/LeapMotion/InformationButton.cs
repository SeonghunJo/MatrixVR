using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InformationButton : MonoBehaviour {

	public GameObject InformationCanvas;
	public GameObject title;
	public GameObject country;
	public GameObject area;
	public GameObject contents;
	public GameObject image;
	public Sprite flag;

	void start()
	{
		flag = Resources.Load<Sprite> (EarthManager.Instance.info_flag_path);
		
		Image tempImage=image.GetComponent<Image> ();
		
		tempImage.sprite = flag;
		
		//tempImage.sprite.texture
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

	public void Clicked()
	{
		flag = Resources.Load<Sprite> (EarthManager.Instance.info_flag_path);
		
		Image tempImage=image.GetComponent<Image> ();
		
		tempImage.sprite = flag;
		
		//tempImage.sprite.texture
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

	public void Pointed()
	{

		flag = Resources.Load<Sprite> (EarthManager.Instance.info_flag_path);

		Image tempImage=image.GetComponent<Image> ();

		tempImage.sprite = flag;

		//tempImage.sprite.texture
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
