using UnityEngine;
using System;
using System.Collections;
using Leap;

public class GesturControllerStreetView : MonoBehaviour
{
    Controller controller;       //립모션 컨트롤러
    GameObject cursorPointer;
    GameObject rigid;

    public GameObject cursorModel;    // 손 포인터 끝 (커서)
    public GameObject target;   // RayCast 충돌위치로  옮겨 표시하는 물체 
    public GameObject leftCamera, rightCamera;
    public GameObject Earth;

    public bool enableScreenTap = true;
    public bool enableKeyTap = true;
    public bool enableSwipe = true;
    public bool enableCircle = true;
    public float swipeSpeed = 100.0f;

    public float maxFov = 130.0f;
    public float minFov = 80.0f;
    public float zoomScale = 0.7f;

    string swipestart = "none";
	Button pointed = null;
	MainMenu MainMenu_pointed = null;


    // Use this for initializatio
    void Start()
    {
        target.renderer.material.color = Color.blue;
        controller = new Controller();  //립모션 컨트롤러 할당 

        if (enableScreenTap)
        { // https://developer.leapmotion.com/documentation/unity/api/Leap.ScreenTapGesture.html
            controller.EnableGesture(Gesture.GestureType.TYPE_SCREEN_TAP);
            controller.Config.SetFloat("Gesture.ScreenTap.MinForwardVelocity", 20.0f); //ScreenTap 조건 
            controller.Config.SetFloat("Gesture.ScreenTap.HistorySeconds", 0.4f); // SHJO 판정시간 0.5초로 바꿈
            controller.Config.SetFloat("Gesture.ScreenTap.MinDistance", 1.0f); // SHJO 최소거리 5에서 1로 바꿈
            controller.Config.Save();
        }
        if (enableKeyTap)
        { // https://developer.leapmotion.com/documentation/unity/api/Leap.KeyTapGesture.html?proglang=unity
            controller.EnableGesture(Gesture.GestureType.TYPE_KEY_TAP);
            controller.Config.SetFloat("Gesture.KeyTap.MinDownVelocity", 20.0f); // SHJO 최소 속도 50에서 30으로 바꿈
            controller.Config.SetFloat("Gesture.KeyTap.HistorySeconds", 0.4f); // SHJO 판정시간  0.1에서 0.3초로 바꿈
            controller.Config.SetFloat("Gesture.KeyTap.MinDistance", 1.0f); // SHJO 최소거리 3에서 1로 바꿈
            controller.Config.Save();
        }
        if (enableSwipe)
        { // https://developer.leapmotion.com/documentation/unity/api/Leap.SwipeGesture.html
            controller.EnableGesture(Gesture.GestureType.TYPE_SWIPE);
            controller.Config.SetFloat("Gesture.Swipe.MinLength", 80.0f); // SHJO Swipe 조건 바꿈 
            controller.Config.SetFloat("Gesture.Swipe.MinVelocity", 200.0f);
            controller.Config.Save();
        }
        if (enableCircle) 
        { // https://developer.leapmotion.com/documentation/unity/api/Leap.CircleGesture.html#id3
            controller.EnableGesture(Gesture.GestureType.TYPE_CIRCLE);
            controller.Config.SetFloat("Gesture.Circle.MinRadius", 10.0f);
            controller.Config.SetFloat("Gesture.Circle.MinArc", 1.0f);
            controller.Config.Save();
        }
    }

    // Update is called once per frame
    void Update()
    {
        Frame frame = controller.Frame();    //frame 
        GestureList gestures = frame.Gestures(); //frame 안의 gesture 인식
        HandList hands = frame.Hands;    //frame 안의 hands 인식
        int num_hands = hands.Count;    //hand의 수

        if (num_hands < 1) // 인식된 손이 없다면
        {
            return;
        }
        else
        {
            Hand left = hands.Leftmost;     //왼쪽손 
            Hand right = hands.Rightmost;   //오른쪽 손

            FingerList fingerList = left.Fingers;  //왼쪽 손의 손가락들!
            Finger leftFinger = fingerList.Frontmost;  //왼손에서 제일 앞에 있는 손가락

            rigid = GameObject.Find("RigidHand(Clone)");
            if (rigid == null)
            {
                Debug.LogWarning("RigidHand is null");
                return;
            }

            cursorPointer = GameObject.Find("RigidHand(Clone)/index/bone3");
            if (cursorPointer == null)
            {
                Debug.LogWarning("CursorPointer is null");
                return;
            }

            //여기에서 립모션 손모양 오븥젝트의 Collision 을 해체하여 Raycast에 충돌 하지 않게 한다.
            Collider[] cols = rigid.GetComponentsInChildren<Collider>();
            for (int i = 0; i < cols.Length; i++)
            {
                cols[i].enabled = false;
            }

            GameObject handController = GameObject.Find("HandController");  //HandCOnroller 오브젝트 접근
            //팁핑 포지션으로 커서를 이동
            if (rigid != null)
            {
                //Debug.Log ("Rigid finded");
                Vector3 temp = Tipping();
                cursorModel.transform.position = temp;
            }

            //손바닥을 UnityScale로 좌표 변환 , Handcontroller TransformPoint로 Transform 형식에 맞게 변환, 이후 왼쪽 카메라 기준으로 월드 스크린으로 변환 
            //Vector2 screenPoint=leftCamera.camera.WorldToScreenPoint (handController.transform.TransformPoint(left.PalmPosition.ToUnityScaled()));
            Vector2 screenPoint = leftCamera.camera.WorldToScreenPoint(cursorPointer.transform.position);
            Ray r = leftCamera.camera.ScreenPointToRay(screenPoint);      // ScreentPoint로부터 Ray를 쏜다
            Debug.DrawRay(r.origin, r.direction * 1000, Color.red);

            RaycastHit hit; //rayCast에서 부딛힌 객체 관리
            
            if (Physics.Raycast(r, out hit, Mathf.Infinity))
            {
				//Debug.Log("hit - ObjName : " + hit.collider.name + 
				         // ", Flag - " +hit.collider.gameObject.tag );
                if(hit.collider != null)
                {
                    target.transform.position = hit.transform.position;
                    if(hit.collider.gameObject.tag == "Button")
                    {
						pointed = hit.collider.gameObject.GetComponent<Button>();
						if(pointed != null)
                        {
							pointed.Pointed();
                        }
                    }
					else if(hit.collider.gameObject.tag == "MainMenu")
					{
						MainMenu_pointed = hit.collider.gameObject.GetComponent<MainMenu>();
						if(MainMenu_pointed != null)
						{
							// TODO : Mouse Enter (HJOOn)
							MainMenu_pointed.Pointed();
						}
					}
				}
			}
			else
			{
				// TODO : Mouse Exit (SHJO)
				if(pointed != null)
                {
					//hit.collider.gameObject.renderer.material.color=Color.red;
					pointed.PointedOut();
					pointed=null;
                }
				else if(MainMenu_pointed !=null)
				{
					MainMenu_pointed.PointedOut();
					MainMenu_pointed = null;
				}
            }

            //립모션 제스쳐 감지 
            for (int i = 0; i < gestures.Count; i++)
            {
                Gesture gesture = gestures[i];
                // Key Tap
                if (gesture.Type == Gesture.GestureType.TYPE_KEY_TAP)
                {
                    KeyTapGesture keyTap = new KeyTapGesture(gesture);
					Debug.Log ("TYPE_KEY_TAP");
					//Debug.Log("TYPE_KEY_TAP - ObjName : " + hit.collider.name + 
					         // ", Flag - " +hit.collider.gameObject.tag + ", Duration : " + keyTap.DurationSeconds.ToString());
                    
					cursorModel.renderer.material.color = Color.blue;                       
                    //if(pointed != null)
					if(hit.collider.gameObject.tag != null)
                    {
                        // TODO : Click (SHJO)
						if(hit.collider.gameObject.tag=="Button")
						{	
                            Debug.Log ("Button Touch");
							pointed.Touch ();
						}
						else if(hit.collider.gameObject.tag=="MainMenu")
						{
							MainMenu_pointed.Clicked();
						}
                    }
                }
                // Screen Tap
                else if (gesture.Type == Gesture.GestureType.TYPE_SCREEN_TAP) 
                {
                    ScreenTapGesture screenTap = new ScreenTapGesture(gesture);
                    Debug.Log("TYPE_SCREEN_TAP");
                    cursorModel.renderer.material.color = Color.red;

					if(hit.collider.gameObject.tag=="Button")
					{
						pointed.Touch ();
					}
                }
                // Swipe
                else if (gesture.Type == Gesture.GestureType.TYPE_SWIPE) 
                {
                  

                }
                // Circle
                else if (gesture.Type == Gesture.GestureType.TYPE_CIRCLE)
                {                   

                }

                // ZOOM IN OUT Motion
                if (num_hands == 2)                                 
                {
                    if (hands[0].IsLeft && gesture.Type == Gesture.GestureType.TYPESWIPE)
                    {
                        Debug.Log("left zoom");
                        SwipeGesture Swipe = new SwipeGesture(gesture);
                        Vector swipeDirection = Swipe.Direction;
                        if (swipeDirection.x < 0)
                        {
                            if (leftCamera.camera.fieldOfView < maxFov)
                            {
                                leftCamera.camera.fieldOfView += zoomScale;
                                rightCamera.camera.fieldOfView += zoomScale;
                            }

                        }
                        else if (swipeDirection.x > 0)
                        {
                            if (leftCamera.camera.fieldOfView > minFov)
                            {
                                leftCamera.camera.fieldOfView -= zoomScale;
                                rightCamera.camera.fieldOfView -= zoomScale;
                            }
                        }
                    }
                    else if ((!hands[0].IsLeft) && gesture.Type == Gesture.GestureType.TYPESWIPE)
                    {
                        Debug.Log("right zoom");
                        SwipeGesture Swipe = new SwipeGesture(gesture);
                        Vector swipeDirection = Swipe.Direction;
                        if (swipeDirection.x > 0)
                        {
                            if (leftCamera.camera.fieldOfView < maxFov)
                            {
                                leftCamera.camera.fieldOfView += zoomScale;
                                rightCamera.camera.fieldOfView += zoomScale;
                            }
                        }
                        else if (swipeDirection.x < 0)
                        {
                            if (leftCamera.camera.fieldOfView > minFov)
                            {
                                leftCamera.camera.fieldOfView -= zoomScale;
                                rightCamera.camera.fieldOfView -= zoomScale;
                            }
                        }
                    }
                } // END OF ZOOM IN GESTURE
            } // END OF GESTURE RECOGNITION LOOP
        
        } // END OF IF
    }

    Vector3 Tipping() // 현재 포인터 끝이 되는 오브젝트의 
    {
        cursorPointer = GameObject.Find("RigidHand(Clone)/index/bone3");       //생선돈 손 모양 객체의 손바닥 오브젝트를 찾는다
        if (cursorPointer != null)
        {
            return cursorPointer.transform.position;  //객체를 찾았다면 객체 위치를 반환
        }
        else
        {
            //Debug.Log ("Find middle finger fail");
            return Vector3.zero;
        }

    }
}



