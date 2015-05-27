using UnityEngine;
using System.Collections;

public class Information : MonoBehaviour
{
    public bool showInfor = false;
    public bool makeText = false;

    OVRInformation inforText;
	void awake()
	{
		Color temp = this.renderer.material.color;
		temp.a = 0.4f;
		this.renderer.material.color = temp;
		this.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);


	}
    void start()
    {
		Color temp = this.renderer.material.color;
		temp.a = 0.4f;
		this.renderer.material.color = temp;
		this.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);

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
        temp.a = 0.2f;
        this.renderer.material.color = temp;
		this.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);


    }
    public void PointedOut()
    {
        inforText.HideScreen();
        Color temp = this.renderer.material.color;
        temp.a = 0.4f;
        this.renderer.material.color = temp;
		this.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
    }
}
