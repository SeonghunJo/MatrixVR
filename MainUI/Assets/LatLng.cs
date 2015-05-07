using UnityEngine;
using System.Collections;

public class LatLng : MonoBehaviour
{

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
    }
    public void SetPosition()
    {
        ID = CreateCube.panoID;

        transform.Rotate(CreateCube._rotation);
        transform.Translate(CreateCube._translate);
        Debug.Log("move object : " + ID);
    }

}
