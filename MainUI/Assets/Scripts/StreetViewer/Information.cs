using UnityEngine;
using System.Collections;

public class Information : MonoBehaviour
{
    public bool showInfor = false;
    public bool makeText = false;

    OVRInformation inforText;

    void start()
    {

    }

    public void Clicked()
    {
        //print("Information Click");
        //if (!showInfor)//Information 보여주기
        //{
        //    inforText.ShowScreen();
        //    showInfor = true;
        //}
        //else
        //{
        //    inforText.HideScreen();
        //    showInfor = false;
        //}
        
    }
    public void Pointed()
    {
        inforText = GameObject.Find("LeapOVRCameraRig").GetComponent<OVRInformation>();
        inforText.ShowScreen();

        Color temp = this.renderer.material.color;
        temp.a = 1f;
        this.renderer.material.color = temp;
        this.transform.localScale = new Vector3(3f, 3f, 3f);


    }
    public void PointedOut()
    {
        inforText.HideScreen();
        Color temp = this.renderer.material.color;
        temp.a = 0.3f;
        this.renderer.material.color = temp;
        this.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
    }
}
