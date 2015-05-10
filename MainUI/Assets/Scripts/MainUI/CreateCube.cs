using UnityEngine;
using System.Collections;

public class PanoramaInfo
{
    public float lat;
    public float lng;
    public string panoid;

    public PanoramaInfo(float lat, float lng, string panoid)
    {
        this.lat = lat;
        this.lng = lng;
        this.panoid = panoid;
    }
}

public class CreateCube : MonoBehaviour
{
    public GameObject obj;
    public GameObject earth;

    public PanoramaInfo[] panoramas = new PanoramaInfo[30];

    //생성되는 객체에서 객체를 translate할 때 사용
    public static Vector3 _rotation;    //회전 vector
    public static Vector3 _translate;   //이동 vector

    //입력된 값을 float로 저장할 변수
    public float Lat;                   //위도 -90~90
    public float Lng;                  //경도 0~360
    public static string panoID;

    // Use this for initialization
    void Start()
    {
        //test value
		panoramas[0] = new PanoramaInfo(31.137713f, 29.979465f, "TXymVghDgJk2ViaA69pNaQ"); // 이집트
		panoramas[1] = new PanoramaInfo(45.432805f, 12.340583f, "CgDdLMNL25XX9atw3dEZog"); // 이탈리아
        panoramas[2] = new PanoramaInfo(37.0f, 127.0f, "zMrHSTO0GCYAAAQINlCkXg");
        for(int i=0 ; i<3; i++) 
        {
            Lat = panoramas[i].lat;
            Lng = panoramas[i].lng;
            panoID = panoramas[i].panoid;

            _rotation = new Vector3(Lat, -Lng, 0.0f);
            _translate = new Vector3(0, 0, -2.5f);

            GameObject child = Instantiate(obj, transform.position, Quaternion.identity) as GameObject;

            child.transform.parent = earth.transform;
            child.GetComponent<StreetviewPoint>().SetPosition();
            
            Debug.Log(Lat + " " + Lng + " " + panoID );
        }
    }

    // Update is called once per frame
    void Update()
    {


    }

    
}
