using UnityEngine;
using System.Collections;
using Leap;

[RequireComponent(typeof(BoxCollider))]

public class EarthScript : MonoBehaviour
{
    public int Speed;		//rotate speed
    public float rotatePosY; //y axis rotate degree
    public float rotatePosX; //x axis rotate degree

    public bool leftClick;
    public bool changeScene;

    public static int chooseWorld = 0; 
    private Vector3 screenPoint;
    private Vector3 offset;

    void Start()
    {
        Speed = 10;
        rotatePosY = 25;
        rotatePosX = 0;
        leftClick = false;
        changeScene = false;

        CameraFade.setFadeOutEndEvent(FadeOutEnd);

    }

    void Update()
    {
        //키보드 좌우 키로 지구 회전
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(Vector3.up, Speed * Time.deltaTime * 5, Space.Self);
            rotatePosY += Speed * Time.deltaTime * 5;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(Vector3.down, Speed * Time.deltaTime * 5, Space.Self);
            rotatePosY -= Speed * Time.deltaTime * 5;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            CameraFade.FadeOutMain();
        }

        if (LeapmotionScript.motionClick)
        {
            var clickPos = Input.mousePosition;
            clickPos.z = 10;
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(clickPos);
            Debug.Log("mousePosition = " + clickPos + "," + worldPos);

            LeapmotionScript.motionClick = false;
        }

  
    }

    void FadeOutEnd()
    {
        Application.LoadLevel(1);
    }

    void OnMouseDown()
    {
        //드래그앤드롭 회전
        /*Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
        pos = Input.mousePosition - pos;
        baseAngle = Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg;
        baseAngle -= Mathf.Atan2(transform.right.y, transform.right.x) * Mathf.Rad2Deg;*/
        //leftClick = true;
        //var clickPos = Input.mousePosition;
        //clickPos.z = 10;

        //Vector3 worldPos = Camera.main.ScreenToWorldPoint(clickPos);
        //Debug.Log(clickPos + "," + worldPos);
        //if (LeapmotionScript.motionClick)
           

        //***********clickPos말고 worldPos를 이용하도록 변경
        //if (rotatePosY >= 170 && rotatePosY <= 220 && clickPos.x >= 155 && clickPos.x <= 310 && clickPos.y >= 200 && clickPos.y <= 280) //아시아
        //{
        //    leftClick = true;
        //    chooseWorld = 1;
            
        //}
        //if (rotatePosY >= 20 && rotatePosY <= 60 && worldPos.x >= -1.0f && worldPos.x <= 1.0f && worldPos.y >= -2.5f && worldPos.y <= 0.0f) //남아메리카
        //{
        //    leftClick = true;
        //    chooseWorld = 2;
        //}

        //screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        //offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(
        //new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
        //Screen.showCursor = false;
    }
    void OnMouseDrag()
    {
        //Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
        //pos = Input.mousePosition - pos;
        //float ang = Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg;
        //transform.rotation = Quaternion.AngleAxis(ang, Vector3.forward);


        //Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        //Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;

        //transform.position = curPosition;
    }

    void onMouseUp()
    {
        //Screen.showCursor = true;
    }

}



