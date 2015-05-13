using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]

public class EarthScript : MonoBehaviour
{
    public int Speed;		//rotate speed

    public void Start()
    {
        Screen.showCursor = true;
        Speed = 10;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(Vector3.up, Speed * Time.deltaTime * 5, Space.Self);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(Vector3.down, Speed * Time.deltaTime * 5, Space.Self);
        }
  
        
    }


}



