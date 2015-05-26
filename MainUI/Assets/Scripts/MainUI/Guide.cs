using UnityEngine;
using System.Collections;

public class Guide : MonoBehaviour
{
    float timeCounter = 0;
    // Use this for initialization
    void Start()
    {

        if (EarthManager.Instance.startGuide != 0)
            Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (EarthManager.Instance.startGuide == 0)
        {
            timeCounter = Time.time;
            if (timeCounter > 15)
            {
                gameObject.transform.localScale -= new Vector3(Time.deltaTime * 50, Time.deltaTime * 40F, 0);
                if (gameObject.transform.localScale.x <= 1F | gameObject.transform.localScale.y <= 1F)
                {
                    Destroy(gameObject);
                    EarthManager.Instance.startGuide = 1;
                }
            }
        }
    }
}
