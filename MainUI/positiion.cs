using UnityEngine;
using System.Collections;
using Leap;

public class positiion : MonoBehaviour {
	Controller controller;
	int speed = 1; //좌우 이동 속도
	public GameObject blockOne;



	// Use this for initializatio
	void Start () {



		Debug.Log ("started ");
		controller = new Controller();
		controller.EnableGesture(Gesture.GestureType.TYPE_SCREEN_TAP);
		controller.EnableGesture(Gesture.GestureType.TYPE_KEY_TAP);
		controller.EnableGesture (Gesture.GestureType.TYPE_SWIPE);
		
		
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
	
	/*
			Gesture.Swipe.MinLength float 150 mm 
			Gesture.Swipe.MinVelocity float 1000 mm/s 
			Gesture.KeyTap.MinDownVelocity float 50 mm/s 
			Gesture.KeyTap.HistorySeconds float 0.1 s 
			Gesture.KeyTap.MinDistance float 3.0 mm 
			Gesture.ScreenTap.MinForwardVelocity float 50 mm/s 
			Gesture.ScreenTap.HistorySeconds float 0.1 s 
			Gesture.ScreenTap.MinDistance float 5.0 mm 

	*/

	
	// Update is called once per frame
	void Update () 
	{
		
		Frame frame = controller.Frame ();    //frame 
		GestureList gestures = frame.Gestures (); //frame 안의 gesture 인식
		HandList hands = frame.Hands;    //frame 안의 hands 인식
		int num_hands = hands.Count;    //hand의 수
		
		Hand left = hands.Leftmost;
		Hand right = hands.Rightmost;
		/*for (int h = 0; h < num_hands; ++h)      //zoomin.zoomout 
		{
			Hand leap_hand = hands [h];
			if(leap_hand.IsLeft&& swipeDirection.x>0)
		}*/  
		//Debug.Log ("Hand normal x- " + left.PalmNormal.x +" y- "+left.PalmNormal.y+" z- " +left.PalmNormal.z + " yaw= " + left.PalmNormal.Yaw );
		//Debug.Log ("Hand pos x- " + left.PalmPosition.x +" y- "+left.PalmPosition.y+" z- " +left.PalmPosition.z+ " yaw= " + left.PalmPosition.Yaw);
		//Debug.Log ("Hand vel x- " + left.PalmVelocity.x +" y- "+left.PalmVelocity.y+" z- " +left.PalmVelocity.z+ " yaw= " + left.PalmVelocity.Yaw);
		//Debug.Log ("Hand wid x- " + left.PalmWidth);
/*
		for(int g = 0; g < frame.Gestures().Count; g++)
		{
			switch (frame.Gestures[g].Type) {
			case Gesture.GestureType.TYPE_CIRCLE:
				Debug.Log ("TYPE_CIRCLE"+gestures.Count);
				break;
			case Gesture.GestureType.TYPE_KEY_TAP:
				Debug.Log ("TYPE_KEY_TAP"+gestures.Count);
				break;
			case Gesture.GestureType.TYPE_SCREEN_TAP:
				Debug.Log ("TYPE_SCREEN_TAP"+gestures.Count);
				break;		
		}
*/


		for(int i =0;i < gestures.Count;i++)
		{
			//Debug.Log ("in for"+gestures.Count);
			//Debug.Log ("GestureCount"+gestures.Count);
			//Debug.Log ("Hand normal x- " + left.PalmNormal.x +" y- "+left.PalmNormal.y+" z- " +left.PalmNormal.z);
			//Debug.Log ("Hand pos - " + left.palmPosition);
			//Debug.Log ("Hand vel - " + left.hand.PalmVelocity);
			//Debug.Log ("Hand width - " + left.PalmWidth);
			Gesture gesture = gestures[i];

				
			if(gesture.Type == Gesture.GestureType.TYPE_KEY_TAP)   //Click
			{
				Debug.Log ("TYPE_KEY_TAP"+gestures.Count);
				blockOne.renderer.material.color = Color.blue;
				click3(gesture);
			}
			if(gesture.Type == Gesture.GestureType.TYPE_SCREEN_TAP)   //Click
			{
				Debug.Log ("TYPE_SCREEN_TAP"+gestures.Count);
				blockOne.renderer.material.color = Color.red;
			}	
			if(gesture.Type == Gesture.GestureType.TYPE_SWIPE)   //Click
			{
				Debug.Log ("TYPE_SWIPE"+gestures.Count);
				blockOne.renderer.material.color = Color.green;
			}		


		}
		
		
	
	}

	void click(Gesture gesture)
	{
		Debug.Log(" click = ");
		Debug.Log("before cube pos = " + blockOne.transform.position);
		Frame frame = controller.Frame ();    //frame 
		GestureList gestures = frame.Gestures (); //frame 안의 gesture 인식
		HandList hands = frame.Hands;    //frame 안의 hands 인식
		int num_hands = hands.Count;    //hand의 수		
		
		
		Hand left = hands.Leftmost;
		Hand right = hands.Rightmost;

		FingerList fingerList= left.Fingers;
		Finger leftFinger=fingerList.Frontmost;

		Vector3 local_tip =leftFinger.TipPosition.ToUnityScaled();

		//Vector3 local_tip =Camera.main.ViewportToScreenPoint (new Vector3 (left.PalmNormal.x,left.PalmNormal.y, left.PalmNormal.z));
	

		local_tip.z  = blockOne.transform.position.z;	




		Debug.Log(" ViewportToWorldPoint = " + local_tip);


		blockOne.transform.position= local_tip;

		Debug.Log("after cube pos = " + blockOne.transform.position);

	
		
	}

	void click2(Gesture gesture)
	{
		Frame frame = controller.Frame ();    //frame 
		GestureList gestures = frame.Gestures (); //frame 안의 gesture 인식
		HandList hands = frame.Hands;    //frame 안의 hands 인식
		int num_hands = hands.Count;    //hand의 수		
		
		Hand left = hands.Leftmost;
		Hand right = hands.Rightmost;
		
		FingerList fingerList= left.Fingers;
		Finger leftFinger=fingerList.Frontmost;
		/*
		HandModel handModel = GetComponent<HandModel>();
		Hand leapHand = handModel.GetLeapHand();
		Vector3 tipPosition = leapHand.Fingers[0].TipPosition.ToUnityScaled();
		//Vector3 tipPosition = handModel.transform.TransformPoint(leapHand.Fingers[0].TipPosition.ToUnityScaled());

		Debug.Log("Unity thumb position = " + GameObject.Find("ThickRigidHand(Clone)/thumb/bone3").transform.position);
		Debug.Log("Leap thumb posiiton = " + tipPosition);

		//thumbTipPosition = handModel.transform.TransformPoint(leapHand.Fingers[0].TipPosition.ToUnityScaled());
		*/

		GameObject controller_ = GameObject.Find ("HandController");

		Vector3 local_tip = leftFinger.TipPosition.ToUnityScaled();
		blockOne.transform.position= controller_.transform.TransformPoint(local_tip);


	}

	void click3(Gesture gesture)
	{
		Frame frame = controller.Frame ();    //frame 
		GestureList gestures = frame.Gestures (); //frame 안의 gesture 인식
		HandList hands = frame.Hands;    //frame 안의 hands 인식
		int num_hands = hands.Count;    //hand의 수		
		
		Hand left = hands.Leftmost;
		Hand right = hands.Rightmost;
		
		FingerList fingerList= left.Fingers;
		Finger leftFinger=fingerList.Frontmost;

		GameObject bone = GameObject.Find ("ThickRigidHand(Clone)/thumb/bone3");


		blockOne.transform.position = bone.transform.position;
		
		
		Debug.Log("Unity thumb position = " + GameObject.Find("ThickRigidHand(Clone)/thumb/bone3").transform.position);
		Debug.Log("Leap thumb posiiton = " + blockOne.transform.position);


	}



}


