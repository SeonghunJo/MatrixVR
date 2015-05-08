using UnityEngine;
using System.Collections;

public class LatLng : MonoBehaviour
{
    public TextAsset imageAsset;
    public GameObject map_pin;

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
        //Application.LoadLevel(1);
    }
    void OnMouseDown()
    {
        Debug.Log("mouse click : " + ID);

        CameraFade.setFadeOutEndEvent(FadeOutEnd);
        CameraFade.FadeOutMain();
    }
    void OnMouseOver()  
    {
        //mouseOver시간을 지정 (checkTime이 경과하면 표지판을 띄움)
        timeSpan += Time.deltaTime;
        if(timeSpan > checkTime)
            Debug.Log("mouse over : " + ID);

        //표지판 그리기
        //Texture2D tex = new Texture2D(2, 2);
        //tex.LoadImage(imageAsset.bytes);
        //GetComponent<Renderer>().material.mainTexture = tex;

    }

    void OnMouseExit()
    {
        Debug.Log("mouse exit : " + ID);
        timeSpan = 0.0f;
    }
    public void SetPosition()
    {
        Lat = CreateCube._rotation.x;
        Lng = -(CreateCube._rotation.y);
        ID = CreateCube.panoID;

        transform.Rotate(CreateCube._rotation);
        transform.Translate(CreateCube._translate);

        Debug.Log("move object : " + Lat + ", " + Lng + ", "+ID);
    }

}
