using UnityEngine;
using System.Collections;

public class LookAt : MonoBehaviour {

    public GameObject target;
	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
        if(target != null)
        {
            transform.LookAt(target.transform.position);
        }

        if(Input.GetKey(KeyCode.U))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z);
        }
        if(Input.GetKey(KeyCode.J))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - 1.0f, transform.position.z);
        }
	}
}
