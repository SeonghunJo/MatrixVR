using UnityEngine;
using System.Collections;

public class StreetviewPoint : MonoBehaviour
{
    public GameObject thumbnail;
    public GameObject earth;

    public float Lat;
    public float Lng;
    public string ID;

    public float timeSpan;
    public float checkTime;


    // Use this for initialization
    void Start()
    {
        timeSpan = 0.0f;
        checkTime = 2.0f;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
            CameraFade.FadeInMain();

    }
    void FadeOutEnd()
    {
        //panoID에 따라 Scene 전환(street view)
        print("FadeOutEnd");
        Manager.Instance.panoramaID = ID;
        print(Manager.Instance.panoramaID);
        Application.LoadLevel("StreetViewer");

    }
    void OnMouseDown()
    {
        Debug.Log("mouse click : " + ID);

        CameraFade.setFadeOutEndEvent(FadeOutEnd);
        CameraFade.FadeOutMain();
    }

    void OnMouseEnter()
    {
        Debug.Log("mouse enter : " + ID);
        GameObject child = Instantiate(thumbnail, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity) as GameObject;

        child.GetComponent<Thumbnails>().SetPosition(ID);

    }
    void OnMouseExit()
    {
        //Destroy(GameObject.FindObjectOfType<Thumbnails>());
        Destroy(GameObject.Find("ThumbnailPrefab(Clone)"));

        Debug.Log("destroy : " + ID);

    }
    public void SetPosition()
    {
        Lat = CreateCube._rotation.x;
        Lng = -(CreateCube._rotation.y);
        ID = CreateCube.panoID;

        transform.Rotate(CreateCube._rotation);
        transform.Translate(CreateCube._translate);

        Debug.Log("move object : " + Lat + ", " + Lng + ", " + ID);
    }

}
