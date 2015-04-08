using UnityEngine;
using System.Collections;
using Leap;

public class TestLeapMotion : MonoBehaviour
{
    Pointable pointable;
    Controller controller;
    Frame frame;
    Vector position;
    float touchDistance;
    Pointable.Zone zone;
    public int Speed; //좌우 이동 속도

    public GameObject Earth;
    public static bool motionClick;
    // Use this for initializatio
    void Start()
    {
        frame = controller.Frame(1);
        pointable = frame.Pointables.Frontmost;
        position = pointable.TipPosition;
        touchDistance = pointable.TouchDistance;
        zone = pointable.TouchZone;

    }

    // Update is called onc
    void Update()
    {
        Debug.Log(touchDistance);
        switch (zone)
        {
            case Pointable.Zone.ZONE_HOVERING :
                Debug.Log("zone_hovering");
                break;
            case Pointable.Zone.ZONE_TOUCHING :
                Debug.Log("zone_touching");
                break;
            case Pointable.Zone.ZONENONE :
                Debug.Log("zone_none");
                break;
            default :
                break;
        }
    }
}
