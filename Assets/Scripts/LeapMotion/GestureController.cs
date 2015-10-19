using UnityEngine;
using System;
using System.Collections;
using Leap;


public class GestureController : MonoBehaviour
{
	Controller controller;       //립모션 컨트롤러
	GameObject cursorPointer;
	GameObject rigid;
	
	public GameObject cursorModel;    // 손 포인터 끝 (커서)
	public GameObject target;   // RayCast 충돌위치로  옮겨 표시하는 물체 
	public GameObject leftCamera, rightCamera;
	public GameObject earth;
	public GameObject clickParticle =null; //클릭 파티클 이펙트
	
	public bool enableScreenTap = true;
	public bool enableKeyTap = true;
	public bool enableSwipe = true;
	public bool enableCircle = true;
	public float swipeSpeed = 100.0f;
	
	public float maxFov = 130.0f;
	public float minFov = 80.0f;
	public float zoomScale = 0.7f;
	
	string swipestart = "none";
	StreetViewPoint StreetView_Pointed = null;

	bool sceneZoomIn = true;  // 신전환시 ZoomIn효과 트리거.
	bool sceneZoomOut = false; // 신전환시 ZoomOut효과 트리거.
	
	// Use this for initializatio
	void Start()
	{
		sceneZoomIn = true;
		sceneZoomOut = false;
		leftCamera.camera.fieldOfView = 178;
		rightCamera.camera.fieldOfView = 178;
		
		target.renderer.material.color = Color.blue;
		controller = new Controller();  //립모션 컨트롤러 할당 
		
		SetGesture (controller);
	}

	// Update is called once per frame
	void Update()
	{
		Frame frame = controller.Frame();    //frame 
		GestureList gestures = frame.Gestures(); //frame 안의 gesture 인식
		HandList hands = frame.Hands;    //frame 안의 hands 인식
		int num_hands = hands.Count;    //hand의 수

		if (sceneZoomIn == true)	//f
		{
			SceneChangeZoom(sceneZoomIn,sceneZoomOut);			
		}

		if (num_hands < 1) // 인식된 손이 없다면
		{
			return;
		}
		else
		{
			FingerList fingerList = hands.Leftmost.Fingers;   //왼쪽 손의 손가락들!
			Finger leftFinger = fingerList.Frontmost;  //왼손에서 제일 앞에 있는 손가락
			
			rigid = GameObject.Find("RigidHand(Clone)");
			if (rigid == null)
			{
				//Debug.Log ("Rigid finded");
				//Vector3 temp = Tipping();  //팁핑 포지션으로 커서를 이동
				//cursorModel.transform.position = temp;
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
			
			//손바닥을 UnityScale로 좌표 변환 , Handcontroller TransformPoint로 Transform 형식에 맞게 변환, 이후 왼쪽 카메라 기준으로 월드 스크린으로 변환 
			Vector2 screenPoint = leftCamera.camera.WorldToScreenPoint(cursorPointer.transform.position);
			Ray r = leftCamera.camera.ScreenPointToRay(screenPoint);      // ScreentPoint로부터 Ray를 쏜다
			Debug.DrawRay(r.origin, r.direction * 1000, Color.red);
			
			RaycastHit hit; //rayCast에서 부딛힌 객체 관리
			
			if (Physics.Raycast(r, out hit, Mathf.Infinity))
			{
				if(hit.collider != null)
				{
					target.transform.position = hit.transform.position;
					if(hit.collider.gameObject.tag == "StreetviewPoint")
					{
						StreetView_Pointed = hit.collider.gameObject.GetComponent<StreetViewPoint>();
						if(StreetView_Pointed != null)
						{
							// TODO : Mouse Enter (SHJO)
							StreetView_Pointed.Pointed() ;
							
						}
					}
				}
			}
			else
			{
				// TODO : Mouse Exit (SHJO)
				if(StreetView_Pointed != null)
				{
					StreetView_Pointed.PointedOut();
					StreetView_Pointed = null;
				}
			}
			
			//립모션 제스쳐 감지 
			for (int i = 0; i < gestures.Count; i++)
			{
				Gesture gesture = gestures[i];
				HandList handsForGesture = gesture.Hands;
				switch (num_hands)                                 
				{
				case 1:
					// Key Tap
					if (gesture.Type == Gesture.GestureType.TYPE_KEY_TAP)
						KeyTap(gesture);
					// Screen Tap
					else if (gesture.Type == Gesture.GestureType.TYPE_SCREEN_TAP) 
						ScreenTap(gesture);
					// Swipe
					else if (gesture.Type == Gesture.GestureType.TYPE_SWIPE) 
						Swipe(gesture);
					// Circle
					else if (gesture.Type == Gesture.GestureType.TYPE_CIRCLE)
						Circle (gesture);
					break;
				case 2:
					ZoomInOut(gesture,handsForGesture);
					break;

				}				
				// ZOOM IN OUT Motion
			
			} // END OF GESTURE RECOGNITION LOOP
			
		} // END OF IF
	}

	//Get Tipping postion
	Vector3 GetTippingPos() // 현재 포인터 끝이 되는 오브젝트의 
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

	//Zoom In Out Effect for Scene changing
	public void SceneChangeZoom(bool zoomIn,bool zoomOut)
	{
		if (zoomIn == true && zoomOut == false)	//f
		{
			//gameObject.transform.localScale -= new Vector3(Time.deltaTime*35F,Time.deltaTime*30F,0);
			leftCamera.camera.fieldOfView -=Time.deltaTime*9F;
			rightCamera.camera.fieldOfView-=Time.deltaTime*9f;
		
			if (leftCamera.camera.fieldOfView < 106) 
			{
				sceneZoomIn=false;	
				sceneZoomOut=true;
			}
		}
		else if(zoomIn==false && zoomOut==true)
		{
			leftCamera.camera.fieldOfView +=Time.deltaTime*9F;
			rightCamera.camera.fieldOfView +=Time.deltaTime*9f;

			if (leftCamera.camera.fieldOfView > 177) 
			{
				sceneZoomOut=false;
				sceneZoomIn=true;
			}
		}
	}	

	//Gesture Event for Keytap 
	void KeyTap(Gesture gesture)
	{
		
		KeyTapGesture keyTap = new KeyTapGesture(gesture);
		Debug.Log("TYPE_KEY_TAP - Duration : " + keyTap.DurationSeconds.ToString());
		
		GameObject particleObj = Instantiate(clickParticle, GetTippingPos () , Quaternion.identity) as GameObject;
		
		Destroy (particleObj,2f);
		
		if(StreetView_Pointed != null)
		{
			// TODO : Click (SHJO)	
			if(sceneZoomOut==true)
				SceneChangeZoom(sceneZoomIn,sceneZoomOut);	

			EarthScript.guideTrigger=false;
			Application.LoadLevel("StreetViewer");
		}
	}
	
	//Gesture Event for ScreenTap
	void ScreenTap(Gesture gesture)
	{
		ScreenTapGesture screenTap = new ScreenTapGesture(gesture);
		print("Screen Tap " + screenTap.Duration.ToString());
		
		if(StreetView_Pointed != null)
		{
			// TODO : Click - Optional (SHJO)
			if(sceneZoomOut==true)
				SceneChangeZoom(sceneZoomIn,sceneZoomOut);
				
			EarthScript.guideTrigger=false;
			Application.LoadLevel("StreetViewer");
		}
	}
	
	//Gesture Event for Swipe
	void Swipe(Gesture gesture)
	{
		SwipeGesture swipe = new SwipeGesture(gesture);
		print("Swipe Speed : " + swipe.Speed.ToString());
		print("Swipe Start : " + swipe.StartPosition.ToString());
		print("Swipe End : " + swipe.Position.ToString());
		
		// TODO : Swipe (SHJO)
		if(swipe.StartPosition.x > swipe.Position.x) // swipe.direction을 써도됨
		{
			earth.transform.Rotate(new Vector3(0, -Time.deltaTime * swipeSpeed, 0));
		}
		else
		{
			earth.transform.Rotate(new Vector3(0, Time.deltaTime * swipeSpeed, 0));
		}
	}
	
	//Gesture Event for Circle
	void Circle(Gesture gesture)
	{
		CircleGesture circleGesture = new CircleGesture(gesture);
		print("Circle");
		
		// TODO : Circle (SHJO)
		if (circleGesture.Pointable.Direction.AngleTo(circleGesture.Normal) <= Math.PI / 2) // Clockwise
		{
			earth.transform.Rotate(new Vector3(0, -Time.deltaTime * swipeSpeed, 0));
		}
		else                                                                                // Counterclockwise
		{
			earth.transform.Rotate(new Vector3(0, Time.deltaTime * swipeSpeed, 0)); 
		}
		
	}
	
	//Gesture Even ZoomInOut
	void ZoomInOut(Gesture gesture,HandList handsForGesture)
	{	
		
		Debug.Log("left zoom");
		SwipeGesture Swipe = new SwipeGesture(gesture);
		Vector swipeDirection = Swipe.Direction;
		float temp = 0;
		if (swipeDirection.x < 0 && handsForGesture[0].IsLeft
		    || swipeDirection.x > 0 && handsForGesture[0].IsRight)
		{
			
			if (leftCamera.camera.fieldOfView < maxFov) 
			{				
				temp = zoomScale;				
			}
			
		}
		else if (swipeDirection.x > 0 && handsForGesture[0].IsLeft
		         || swipeDirection.x < 0 && handsForGesture[0].IsRight)
		{
			if (leftCamera.camera.fieldOfView > minFov) 
			{
				temp = zoomScale * -1; ;				
			}			
		}		

		leftCamera.camera.fieldOfView += temp;
		rightCamera.camera.fieldOfView += temp;	

	}
	//Enable LeapMotion Gesture
	void SetGesture(Controller controller)
	{
		if (enableScreenTap)
		{ // https://developer.leapmotion.com/documentation/unity/api/Leap.ScreenTapGesture.html
			controller.EnableGesture(Gesture.GestureType.TYPE_SCREEN_TAP);
			controller.Config.SetFloat("Gesture.ScreenTap.MinForwardVelocity", 30.0f); //ScreenTap 조건 30초로 바꿈
			controller.Config.SetFloat("Gesture.ScreenTap.HistorySeconds", 0.4f); // SHJO 판정시간 0.5초로 바꿈
			controller.Config.SetFloat("Gesture.ScreenTap.MinDistance", 1.0f); // SHJO 최소거리 5에서 3로 바꿈
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
			controller.Config.SetFloat("Gesture.Swipe.MinLength", 60.0f); // SHJO Swipe 조건 바꿈 
			controller.Config.SetFloat("Gesture.Swipe.MinVelocity", 100.0f);
			controller.Config.Save();
		}
		if (enableCircle) 
		{ // https://developer.leapmotion.com/documentation/unity/api/Leap.CircleGesture.html#id3
			controller.EnableGesture(Gesture.GestureType.TYPE_CIRCLE);
			controller.Config.SetFloat("Gesture.Circle.MinRadius", 10.0f);
			controller.Config.SetFloat("Gesture.Circle.MinArc", 1.0f);
			controller.Config.Save();
			//하하하 
		}
	}
	
}




