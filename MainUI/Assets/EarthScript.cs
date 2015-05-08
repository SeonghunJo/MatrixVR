using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]

public class EarthScript : MonoBehaviour
{
    public int Speed;		//rotate speed
    public float rotatePosY; //y axis rotate degree
    public float rotatePosX; //x axis rotate degree

    public void Start()
    {
        Speed = 10;
        rotatePosY = 25;
        rotatePosX = 0;
        
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(Vector3.up, Speed * Time.deltaTime * 5, Space.Self);
            rotatePosY += Speed * Time.deltaTime * 5;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(Vector3.down, Speed * Time.deltaTime * 5, Space.Self);
            rotatePosY -= Speed * Time.deltaTime * 5;
        }

  
    }


}



