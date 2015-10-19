using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]

//Mouse test
public class EarthScript : MonoBehaviour
{
    public int Speed;		//rotate speed

    public void Start()
    {
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

        /*
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.Rotate(Vector3.left, Speed * Time.deltaTime * 5, Space.Self);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.Rotate(Vector3.right, Speed * Time.deltaTime * 5, Space.Self);
        }
        */
        
    }


}



