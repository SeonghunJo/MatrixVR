using UnityEngine;
using System.Collections;
using Leap;

public class LeapmotionScript : MonoBehaviour
{
    Controller controller;
    public int Speed; //좌우 이동 속도
    
    public GameObject Earth;
    public static bool motionClick;
    // Use this for initializatio
    void Start()
    {
        controller = new Controller();
        motionClick = false;

        controller.EnableGesture(Gesture.GestureType.TYPE_SCREEN_TAP);
        controller.EnableGesture(Gesture.GestureType.TYPE_KEY_TAP);
        controller.EnableGesture(Gesture.GestureType.TYPE_SWIPE);
        controller.Config.SetFloat("Gesture.Swipe.MinLength", 200.0f);
        controller.Config.SetFloat("Gesture.Swipe.MinVelocity", 750f);
        controller.Config.Save();
        controller.Config.SetFloat("Gesture.KeyTap.MinDownVelocity", 30.0f);
        controller.Config.SetFloat("Gesture.KeyTap.HistorySeconds", .1f);
        controller.Config.SetFloat("Gesture.KeyTap.MinDistance", 1.0f);
        controller.Config.Save();
        controller.Config.SetFloat("Gesture.ScreenTap.MinForwardVelocity", 30.0f);
        //controller.Config.SetFloat("Gesture.ScreenTap.HistorySeconds", .5f);
        controller.Config.SetFloat("Gesture.ScreenTap.MinDistance", 2.0f);
        controller.Config.Save();


    }

    // Update is called onc
    void Update()
    {
        Frame frame = controller.Frame();
        GestureList gestures = frame.Gestures();
        Hand hand = frame.Hands.Frontmost;
        for (int i = 0; i < gestures.Count; i++)
        {
            
            Gesture gesture = gestures[i];

            if (gesture.Type == KeyTapGesture.ClassType())   //click event
            {
                KeyTapGesture keytapGesture = new KeyTapGesture(gesture);

                Pointable tappingPointable = keytapGesture.Pointable;
                Vector3 position = new Vector3(tappingPointable.TipPosition.x, tappingPointable.TipPosition.y, -tappingPointable.TipPosition.z);
                position.z = camera.farClipPlane;
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(position);
                
                Debug.Log("LeapmotionPosition = " + position + "," + worldPos);

                motionClick = true;
            }
            else if (gesture.Type == Gesture.GestureType.TYPESWIPE)  //swipe event
            {
                SwipeGesture Swipe = new SwipeGesture(gesture);
                Vector swipeDirection = Swipe.Direction;
                float currentSwipeSpeed = Swipe.Speed;
                if (swipeDirection.x < 0)
                {
                    Debug.Log("Right + "+ currentSwipeSpeed);
                    //float key = Input.GetAxis("Horizontal");
                    //Earth.transform.Translate(Vector3.right * amtMove * -swipeDirection.x, Space.World);
                    Earth.transform.Rotate(Vector3.down, Speed * Time.deltaTime, Space.Self);

                }
                else if (swipeDirection.x > 0)
                {
                    Debug.Log("Left + " + currentSwipeSpeed);
                    //float key = Input.GetAxis("Horizontal");
                    Earth.transform.Rotate(Vector3.up, Speed * Time.deltaTime, Space.Self);
                    //rotatePosY += Speed * Time.deltaTime * 5;

                }

            }
        }
    }
}
