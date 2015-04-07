using UnityEngine;
using System.Collections;
using Leap;

public class NewBehaviourScript : MonoBehaviour {
	Controller controller;
	int speed = 1; //좌우 이동 속도
	public Transform target;

	public GameObject blockOne;
	public GameObject blockTwo;
	public GameObject blockThree;
	public GameObject blockFour;
	// Use this for initializatio
	void Start () {
	
		controller = new Controller();

		controller.EnableGesture(Gesture.GestureType.TYPE_SCREEN_TAP);
		controller.EnableGesture(Gesture.GestureType.TYPE_KEY_TAP);
		controller.EnableGesture (Gesture.GestureType.TYPE_SWIPE);
		controller.Config.SetFloat("Gesture.Swipe.MinLength", 200.0f);
		controller.Config.SetFloat("Gesture.Swipe.MinVelocity", 750f);
		controller.Config.Save();
		controller.Config.SetFloat("Gesture.KeyTap.MinDownVelocity", 40.0f);
		controller.Config.SetFloat("Gesture.KeyTap.HistorySeconds", .2f);
		controller.Config.SetFloat("Gesture.KeyTap.MinDistance", 1.0f);
		controller.Config.Save();
		controller.Config.SetFloat("Gesture.ScreenTap.MinForwardVelocity", 30.0f);
		//controller.Config.SetFloat("Gesture.ScreenTap.HistorySeconds", .5f);
		controller.Config.SetFloat("Gesture.ScreenTap.MinDistance", 2.0f);
		controller.Config.Save();

	  
	}
	
	// Update is called onc
	void Update () {
		Frame frame = controller.Frame ();
		GestureList gestures = frame.Gestures ();

		//HandList hands = frame.Hands;
		for(int i =0;i < gestures.Count;i++)
		{
			var clickPos = Input.mousePosition;
			Debug.Log("position = " + clickPos );
			Gesture gesture = gestures[i];
			/*Gesture gesture2 = gestures[1];
			if(gesture.Type == Gesture.GestureType.TYPESWIPE && gesture2.Type == Gesture.GestureType.TYPESWIPE)
			{
				Hand leftmost = hands.Leftmost;
				Hand rightmost = hands.Rightmost;
				
				SwipeGesture Swipe = new SwipeGesture(gesture);
				SwipeGesture Swipe2 = new SwipeGesture(gesture);
				Vector swipeDirection = Swipe.Direction;
				Vector swipeDirection2 = Swipe.Direction;
				if(swipeDirection.x <0){)
			   //else if(hand.IsRight swipeDirection.x <0)	
			}*/
			if(gesture.Type == Gesture.GestureType.TYPEKEYTAP)
			{
				KeyTapGesture keytapGesture = new KeyTapGesture(gesture);
				///Debug.Log ("ktap");
				Pointable tappingPointable = keytapGesture.Pointable;
				Vector3 position = new Vector3(tappingPointable.TipPosition.x,tappingPointable.TipPosition.y,-tappingPointable.TipPosition.z);
				Debug.Log("LeapMotion position = " + position);
				position.z =  camera.farClipPlane;
				Vector3 worldPos = Camera.main.ScreenToWorldPoint(position);
				Debug.Log("mousePosition = " + position + "," + worldPos);
				RaycastHit hit;
				
				if(Physics.Raycast(transform.position,worldPos,out hit,position.z))
					
				{
					
					target.position = hit.point; // 타겟을 레이캐스트가 충돌된 곳으로 옮긴다.
					
				}

				//blockOne.renderer.material.color = Color.blue;
				//blockTwo.renderer.material.color = Color.blue;
				//blockThree.renderer.material.color = Color.blue;
				//blockFour.renderer.material.color = Color.blue;
			}
			else if(gesture.Type == Gesture.GestureType.TYPESWIPE)
			{
				SwipeGesture Swipe = new SwipeGesture(gesture);
				Vector swipeDirection = Swipe.Direction;
				//Vector currentSwipePosition = swipe.Position;
	
				if(swipeDirection.x <0){
					Debug.Log ("Right");
					float amtMove = speed;
					//float key = Input.GetAxis("Horizontal");
					Vector swipeStart = Swipe.StartPosition;
					blockOne.transform.Translate(Vector3.right * amtMove * -swipeDirection.x , Space.World);
					blockTwo.transform.Translate(Vector3.right * amtMove * -swipeDirection.x , Space.World);
					blockThree.transform.Translate(Vector3.right * amtMove * -swipeDirection.x , Space.World);
					blockFour.transform.Translate(Vector3.right * amtMove * -swipeDirection.x , Space.World);

				}
				else if(swipeDirection.x>0){
					Debug.Log ("Left");
					float amtMove = speed;
					//float key = Input.GetAxis("Horizontal");
					blockOne.transform.Translate(Vector3.right * amtMove * -swipeDirection.x , Space.World);
					blockTwo.transform.Translate(Vector3.right * amtMove * -swipeDirection.x , Space.World);
					blockThree.transform.Translate(Vector3.right * amtMove * -swipeDirection.x , Space.World);
					blockFour.transform.Translate(Vector3.right * amtMove * -swipeDirection.x , Space.World);
				}

			}
			/*else if(gesture.Type == Gesture.GestureType.TYPESCREENTAP) 
			{
				ScreenTapGesture screentap = new ScreenTapGesture(gesture);
					Vector pokeLocation = screentap.Position;
				Debug.Log ("tap");
				blockOne.renderer.material.color = Color.blue;
				blockTwo.renderer.material.color = Color.blue;
				blockThree.renderer.material.color = Color.blue;
				blockFour.renderer.material.color = Color.blue;
				//DestroyObject(blockOne);
			}*/
			//else if(hand.IsRight swipeDirection.x <0)	
		

		}	
	}
}
