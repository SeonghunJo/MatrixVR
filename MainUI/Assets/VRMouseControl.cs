using UnityEngine;
using System.Collections;

public class VRMouseControl : MonoBehaviour {

    public GameObject targetCam;
    // Use this for initialization
    void Start()
    {
        targetCam = transform.parent.gameObject;
    }


    // Update is called once per frame
    void Update()
    {
        transform.LookAt(targetCam.transform);

        float localX = transform.localPosition.x;
        float localY = transform.localPosition.y;

        float h = (localX += (Input.GetAxis("Mouse X") * 0.05f));
        float v = (localY += (Input.GetAxis("Mouse Y") * 0.05f));
        transform.localPosition = new Vector3(h, v, transform.localPosition.z); 

    }

    void OnMouseDown()
    {
        Debug.Log("click");
    }

    void mouseMoveEvent()
    {

    }
}

//public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
//public RotationAxes axes = RotationAxes.MouseXAndY;
//public float sensitivityX = 15F;
//public float sensitivityY = 15F;

//public float minimumX = -360F;
//public float maximumX = 360F;

//public float minimumY = -60F;
//public float maximumY = 60F;

//float rotationY = 0F;

//void Update()
//{
//    if (axes == RotationAxes.MouseXAndY)
//    {
//        float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;

//        rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
//        rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

//        transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
//    }
//    else if (axes == RotationAxes.MouseX)
//    {
//        transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
//    }
//    else
//    {
//        rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
//        rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

//        transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
//    }
//}

//void Start()
//{
//    // Make the rigid body not change rotation
//    if (rigidbody)
//        rigidbody.freezeRotation = true;
//}