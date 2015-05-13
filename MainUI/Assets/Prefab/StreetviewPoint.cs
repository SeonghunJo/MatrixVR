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

        child = Instantiate(thumbnail, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity) as GameObject;

        child.GetComponent<Thumbnails>().SetPosition(ID);

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
        CameraFade.setFadeOutEndEvent(FadeOutEnd);
        CameraFade.FadeOutMain();
    }

    void OnMouseEnter()
    {
        Debug.Log("mouse enter : " + ID);
        child = Instantiate(thumbnail, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity) as GameObject;

        child.GetComponent<Thumbnails>().SetPosition(ID);

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
