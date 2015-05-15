using UnityEngine;
using System.Collections;

public class StreetviewPoint : MonoBehaviour
{
    public GameObject thumbnail;
    public GameObject earth;
    public GameObject child;

    public float Lat;
    public float Lng;
    public string ID;


    // Use this for initialization
    void Start() {


    }

    // Update is called once per frame
    void Update()
    {

    }
    void FadeOutEnd()
    {
        //panoID에 따라 Scene 전환(street view)
        Manager.Instance.panoramaID = ID;
        Application.LoadLevel("StreetViewer");

    }

	public void Clicked()
	{
		CameraFade.setFadeOutEndEvent(FadeOutEnd);
		CameraFade.FadeOutMain();
	}

    void OnMouseDown()
    {
		
		Debug.Log (ID);

        CameraFade.setFadeOutEndEvent(FadeOutEnd);
        CameraFade.FadeOutMain(); 
    }

	
	public void Pointed ()
	{

	}

	public void StereoDrawTexture(float X, float Y, float wX, float hY, ref Texture image, Color color)
	{
				StereoDrawTexture ((int)(X * PixelWidth), 
		                   (int)(Y * PixelHeight),
		                   (int)(wX * PixelWidth),
		                   (int)(hY * PixelHeight),
		                   ref image, color);
	}

    void OnMouseEnter()
    {
        Debug.Log("mouse enter : " + ID);

	
        /*child = Instantiate(thumbnail, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity) as GameObject;

        child.GetComponent<Thumbnails>().SetPosition(ID);
*/
    }

	public void PointedOut()
	{
		Destroy(child);
	}

    void OnMouseExit()
    {
        Destroy(child);
        Debug.Log("destroy : " + ID);
    }
    public void SetPosition()
    {
        Lat = CreatePoint._rotation.x;
        Lng = -(CreatePoint._rotation.y);
        ID = CreatePoint.panoID;

        transform.Rotate(CreatePoint._rotation);
        transform.Translate(CreatePoint._translate);
    }

}
