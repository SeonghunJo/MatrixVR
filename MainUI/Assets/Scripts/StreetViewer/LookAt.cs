using UnityEngine;
using System.Collections;

public class LookAt : MonoBehaviour {

    public GameObject target;

	// Use this for initialization
    void Start () 
    {
	    
	}
	
	// Update is called once per frame
	void Update () 
    {
        if(target != null)
        {
            transform.LookAt(target.transform.position);
        }
        
        if(Input.GetKey(KeyCode.U))
        {
            if(270 < transform.rotation.eulerAngles.x || transform.rotation.eulerAngles.x + 10.0f < 90)
                transform.RotateAround(target.collider.bounds.center, new Vector3(1.0f, 0, 0.0f), 5.0f);
        }
        if(Input.GetKey(KeyCode.J)) 
        {
            if (270 < transform.rotation.eulerAngles.x - 10.0f || transform.rotation.eulerAngles.x < 90)
                transform.RotateAround(target.collider.bounds.center, new Vector3(-1.0f, 0, 0.0f), 5.0f);
        }
	}
}
