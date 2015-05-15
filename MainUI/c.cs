using UnityEngine;
using System.Collections;
using Leap;

public class Position : MonoBehaviour {
	Controller controller;       //립모션 컨트롤러
	int speed = 1; //좌우 이동 속도
	GameObject cursorPointer;
	GameObject rigid;
	
	public GameObject cursorModel;    // 손 포인터 끝 (커서)
	public GameObject target;   // RayCast 충돌위치로  옮겨 표시하는 물체 
	public GameObject leftCamera,rightCamera ;
	public GameObject Earth;
	
	
	
	public string swipestart= "none";
	
	Vector de;
	
	// Use this for initializatio
	void Start () {
		
		Debug.Log ("started ");
		
		target.renderer.material.color = Color.black; 
		
		controller = new Controller();        //립모션 컨트롤러 할당 
		
		// 스크린탭,키탭,스와이프 모션 제스쳐로 인식하게 설정
		controller.EnableGesture(Gesture.GestureType.TYPE_SCREEN_TAP);
		controller.EnableGesture(Gesture.GestureType.TYPE_KEY_TAP);
		controller.EnableGesture (Gesture.GestureType.TYPE_SWIPE);
		
		//각 제스처 발동 조건 설정
		controller.Config.SetFloat("Gesture.Swipe.MinLength", 100.0f); //Swipe 조건 
		controller.Config.SetFloat("Gesture.Swipe.MinVelocity", 1f);
		controller.Config.Save();
		controller.Config.SetFloat("Gesture.KeyTap.MinDownVelocity", 50.0f); //Click KeyTap 조건 
		controller.Config.SetFloat("Gesture.KeyTap.HistorySeconds", 0.1f);
		controller.Config.SetFloat("Gesture.KeyTap.MinDistance", 3.0f);
		controller.Config.Save();
		controller.Config.SetFloat("Gesture.ScreenTap.MinForwardVelocity", 50.0f); //ScreenTap 조건 
		controller.Config.SetFloat("Gesture.ScreenTap.HistorySeconds", 0.1f);
		controller.Config.SetFloat("Gesture.ScreenTap.MinDistance", 5.0f);
		controller.Config.Save();
	}
	
	// Update is called once per frame
	void Update () 
	{
		Frame frame = controller.Frame ();    //frame 
		GestureList gestures = frame.Gestures (); //frame 안의 gesture 인식
		HandList hands = frame.Hands;    //frame 안의 hands 인식
		int num_hands = hands.Count;    //hand의 수		
		
		Hand left = hands.Leftmost;     //왼쪽손 
		Hand right = hands.Rightmost;   //오른쪽 손
		
		FingerList fingerList= left.Fingers;  //왼쪽 손의 손가락들!
		Finger leftFinger=fingerList.Frontmost;  //왼손에서 제일 앞에 있는 손각락
		
		
		rigid = GameObject.Find ("RigidHand(Clone)");
		cursorPointer = GameObject.Find ("RigidHand(Clone)/index/bone3");
		
		//여기에서 립모션 손모양 오븥젝트의 ollision 을 해체하여 Raycast에 충돌 하지 않게 한다.
		if(rigid != null){
			//Debug.Log("find");
			Collider[] cols = rigid.GetComponentsInChildren<Collider>();
			
			for(int i=0; i<cols.Length ; i++)
			{
				cols[i].enabled=false;
			}
			
		}
		GameObject handController = GameObject.Find ("HandController");        //HandCOnroller 오브젝트 접근
		
		
		//팁핑 포지션으로 커서를 이동
		if( rigid != null) 
		{
			//Debug.Log ("Rigid finded");
			Vector3 temp = Tipping ();
			cursorModel.transform.position = temp;
		}
		Vector2 screenPoint=leftCamera.camera.WorldToScreenPoint (cursorPointer.transform.position);
		
		//손바닥을 UnityScale로 좌표 변환 , Handcontroller TransformPoint로 Transform 형식에 맞게 변환, 이후 왼쪽 카메라 기준으로 월드 스크린으로 변환 
		//Vector2 screenPoint=leftCamera.camera.WorldToScreenPoint (handController.transform.TransformPoint(left.PalmPosition.ToUnityScaled()));
		Ray r = leftCamera.camera.ScreenPointToRay (screenPoint);      //ScreentPoint로부터 Ray를 쏜다
		
		
		Debug.DrawRay (r.origin, r.direction * 1000,Color.red); // 손바닥기준 , 손바닥이 가리키는 방향으로, 날라간다})
		
		RaycastHit hit; //rayCast에서 부딛힌 객체 관리
		
		if (Physics.Raycast (r, out hit, Mathf.Infinity)) 
		{
			target.transform.position=hit.transform.position;
			StreetviewPoint pointed = GameObject.Find (hit.collider.gameObject.name).GetComponent("StreetviewPoint") as StreetviewPoint ;
			
			if(pointed != null  )
			{
				if(Manager.Instance.thumbnailID == null)
				{
					Manager.Instance.thumbnailID = pointed.panoID;
					pointed.Pointed();
				}
			}
			
		}
		
		//립모션 제스쳐 감지 
		for(int i =0;i < gestures.Count;i++)  
		{
			Gesture gesture = gestures[i];				
			if(gesture.Type == Gesture.GestureType.TYPE_KEY_TAP)   //손가락 까닥 클릭모션 
			{
				Debug.Log ("TYPE_KEY_TAP"+gestures.Count);
				cursorModel.renderer.material.color = Color.blue;
				//click2(gesture);
				
				if (Physics.Raycast (r, out hit, Mathf.Infinity)) 
				{
					Debug.Log ("Hit On Tap = " + hit.collider.gameObject.name);
					target.transform.position=hit.transform.position;
					//StreetviewPoint clicked = GameObject.Find (hit.collider.gameObject.name).GetComponent("StreetviewPoint") as StreetviewPoint ;
					
					StreetviewPoint clicked = hit.collider.gameObject.GetComponent("StreetviewPoint")  as StreetviewPoint ; 
					if(clicked != null)
					{
						clicked.Clicked();
					}
				}
			}
			if(gesture.Type == Gesture.GestureType.TYPE_SCREEN_TAP)   //손가락 쭉뼈서 찌르는 모션 
			{
				Debug.Log ("TYPE_SCREEN_TAP"+gestures.Count);
				cursorModel.renderer.material.color = Color.red;
			}	
			if(gesture.Type == Gesture.GestureType.TYPE_SWIPE)   //스와이프 모션 
			{					
				swipe(gesture);
				
				if (swipestart == "right")
				{
					Debug.Log ("right SWIPE"+gestures.Count);
					Earth.transform.Rotate(new Vector3(0,-Time.deltaTime*40,0));
				}
				else if (swipestart == "left")
				{
					Debug.Log ("left SWIPE"+gestures.Count);
					Earth.transform.Rotate(new Vector3(0,Time.deltaTime*40,0)); 
				}
				
			}
			
			if(num_hands==2)
			{
				HandList hands2 = gesture.Hands;
				
				if (hands2[0].IsLeft && gesture.Type == Gesture.GestureType.TYPESWIPE)
				{
					
					Debug.Log("left zoom");
					SwipeGesture Swipe = new SwipeGesture(gesture);
					Vector swipeDirection = Swipe.Direction;
					if (swipeDirection.x < 0)
					{
						
						if(leftCamera.camera.fieldOfView<130)
						{
							leftCamera.camera.fieldOfView += 0.7f;
							rightCamera.camera.fieldOfView += 0.7f;
						}						
						
					}
					else if (swipeDirection.x > 0)
					{
						if(leftCamera.camera.fieldOfView>80)
						{
							leftCamera.camera.fieldOfView -= 0.7f;
							rightCamera.camera.fieldOfView -= 0.7f;
						}
					}
				}
				else if ((!hands2[0].IsLeft) && gesture.Type == Gesture.GestureType.TYPESWIPE)
				{
					Debug.Log("right zoom");
					SwipeGesture Swipe = new SwipeGesture(gesture);
					Vector swipeDirection = Swipe.Direction;
					if (swipeDirection.x > 0)
					{
						if(leftCamera.camera.fieldOfView<130)
						{
							leftCamera.camera.fieldOfView += 0.7f;
							rightCamera.camera.fieldOfView += 0.7f;
						}
						
						
					}
					else if (swipeDirection.x < 0)
					{
						if(leftCamera.camera.fieldOfView>80)
						{
							leftCamera.camera.fieldOfView -= 0.7f;
							rightCamera.camera.fieldOfView -= 0.7f;	
						}				
						
					}
				}//Time.deltaTime*smooth*/
			}
			
			
		}		
		
	}
	
	void swipe(Gesture gesture)
	{
		SwipeGesture Swipe = new SwipeGesture(gesture);
		Vector swipeDirection = Swipe.Direction;
		de = swipeDirection;
		if (swipeDirection.x < 0)
		{
			swipestart = "right";
			
		}
		else if (swipeDirection.x > 0)
		{
			swipestart = "left";
		}
	}
	
	void click(Gesture gesture)
	{
		Frame frame = controller.Frame ();    //frame 
		GestureList gestures = frame.Gestures (); //frame 안의 gesture 인식
		HandList hands = frame.Hands;    //frame 안의 hands 인식
		int num_hands = hands.Count;    //hand의 수		
		
		Hand left = hands.Leftmost;
		Hand right = hands.Rightmost;
		
		FingerList fingerList= left.Fingers;
		Finger leftFinger=fingerList.Frontmost;
		
		//Debug.Log("before cube pos = " + blockOne.transform.position);
		Vector3 local_tip =leftFinger.TipPosition.ToUnityScaled();
		//Vector3 local_tip =Camera.main.ViewportToScreenPoint (new Vector3 (left.PalmNormal.x,left.PalmNormal.y, left.PalmNormal.z));
		local_tip.z  = target.transform.position.z;	
		Debug.Log(" ViewportToWorldPoint = " + local_tip);
		target.transform.position= local_tip;
		Debug.Log("after cube pos = " + target.transform.position);
		
		
	}
	
	void click2(Gesture gesture)
	{
		Debug.Log( "Click2");
		
		Frame frame = controller.Frame ();    //frame 
		GestureList gestures = frame.Gestures (); //frame 안의 gesture 인식
		HandList hands = frame.Hands;    //frame 안의 hands 인식
		int num_hands = hands.Count;    //hand의 수			
		Hand left = hands.Leftmost;
		Hand right = hands.Rightmost;
		
		FingerList fingerList= left.Fingers;
		Finger leftFinger=fingerList.Frontmost;		
		
		GameObject controller_ = GameObject.Find ("HandController");
		Vector3 local_tip = leftFinger.TipPosition.ToUnityScaled ();
		//target.transform.position=leftCamera.camera.
		//Debug.Log( "target.transform.position ;"+target.transform.position );
		
	}
	
	void click3(Gesture gesture)
	{
		Frame frame = controller.Frame ();    //frame 
		GestureList gestures = frame.Gestures (); //frame 안의 gesture 인식	
		
		//GameObject bone = GameObject.Find ("ThickRigidHand(Clone)/thumb/bone3");	 
		//target.transform.position = bone.transform.position;		
		//Debug.Log("Unity thumb position = " + GameObject.Find("ThickRigidHand(Clone)/thumb/bone3").transform.position);
		//Debug.Log("Leap thumb posiiton = " + blockOne.transform.position);
		
	}
	
	Vector3 Tipping()
	{
		Frame frame = controller.Frame ();    //frame 
		GestureList gestures = frame.Gestures (); //frame 안의 gesture 인식
		HandList hands = frame.Hands;    //frame 안의 hands 인식
		int num_hands = hands.Count;    //hand의 수		
		
		Hand left = hands.Leftmost;
		Hand right = hands.Rightmost;
		
		FingerList fingerList= left.Fingers;
		Finger leftFinger=fingerList.Frontmost;
		
		//Debug.Log ("Tipping");
		cursorPointer = GameObject.Find ("RigidHand(Clone)/index/bone3");       //생선돈 손 모양 객체의 손바닥 오브젝트를 찾는다
		if (cursorPointer != null) 
		{
			//Debug.Log ("Find middle finger Suceded");
			return cursorPointer.transform.position;  //객체를 찾았다면  손바닥 중심 위치를 반환
		} 
		else
		{	
			//Debug.Log ("Find middle finger fail");
			return Vector3.zero;
		}
		
	}
	
}



