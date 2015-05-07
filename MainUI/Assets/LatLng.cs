using UnityEngine;
using System.Collections;

public class LatLng : MonoBehaviour
{
    public TextAsset imageAsset;
    public GameObject map_pin;
    public string ID;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnMouseDown()
    {
        Debug.Log("mouse click : " + ID);
    }
    void OnMouseOver()
    {
        //표지판 그리기
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(imageAsset.bytes);
        GetComponent<Renderer>().material.mainTexture = tex;


    }

    public void SetPosition()
    {
        ID = CreateCube.panoID;

        transform.Rotate(CreateCube._rotation);
        transform.Translate(CreateCube._translate);
        Debug.Log("move object : " + ID);
    }

}
