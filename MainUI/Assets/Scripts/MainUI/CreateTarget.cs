using UnityEngine;
using System.Collections;

public class PanoramaInfo
{
    public float lat;
    public float lng;
    public string panoid;
	public bool impo;

    public PanoramaInfo(float lat, float lng, string panoid, bool impo)
    {
        this.lat = lat;
        this.lng = lng;
        this.panoid = panoid;
		this.impo = impo;
    }
}

public class CreateTarget : MonoBehaviour
{
	public GameObject NonImpObj;
	public GameObject ImpObj;
    public GameObject earth;

    public PanoramaInfo[] panoramas = new PanoramaInfo[33];

    //생성되는 객체에서 객체를 translate할 때 사용
    public static Vector3 _rotation;    //회전 vector
    public static Vector3 _translate;   //이동 vector

    //입력된 값을 float로 저장할 변수
    public float Lat;                   //위도 -90~90
    public float Lng;                  //경도 0~360
    public static string panoID;
	public bool impo;

    // Use this for initialization
    void Start()
    {
        //test value
		panoramas[0] = new PanoramaInfo(50.059139f, -122.958407f, "Zzl28rqGJgaL2IdkUleP8A", true); // 캐나다, 브리티시컬럼비아주휘슬러-블랙콤 스키 리조트
		panoramas[1] = new PanoramaInfo(21.441916f, -157.808187f, "UPGaj__Xj9yHswn5Ul0yKg", true); // 미국, Heeia State Park
		panoramas[2] = new PanoramaInfo(36.065096f, -112.137107f, "Fa-wHCWazJG6bn7ZjISQCA", true); // 미국, Grand Canyon
		panoramas[3] = new PanoramaInfo(37.172210f, -93.322539f, "ayOK2SvpqYzlk4rm1Q4KPQ", false); // 미국, Mizumoto Japanese Stroll Garden
		panoramas[4] = new PanoramaInfo(38.907877f, -77.072374f, "r_4YsN868xJX25PKfCjXZQ", false); // 미국, Georgetown University
		panoramas[5] = new PanoramaInfo(-3.825792f, -32.396424f, "rfgb-QoM16gAAAQY_-2qjg", false); // 미국, Catlin Seaview Survey
		panoramas[6] = new PanoramaInfo(-0.396520f, -90.292014f, "QY2gQ6TM6M3DECEX7Ie3xQ", false); // 에콰도르, Galapagos
		panoramas[7] = new PanoramaInfo(-3.852043f, -32.442343f, "-dqeDyeJ_jVbfMxRF6jUJw", false); // 브라질, Mirante
		panoramas[8] = new PanoramaInfo(-23.545225f, -46.474271f, "JUyCJZzn1kQ6iX9ClR4HYg", false); // 브라질, Arena Corinthians
		panoramas[9] = new PanoramaInfo(68.509006f, 27.481845f, "GtnAyp0MCbYAAAQZLDcQIA", false); // 핀란드, Northern Lights
		panoramas[10] = new PanoramaInfo(66.543346f, 25.848498f, "hEuDBjnLcagAAAQZA4Rr0g", false); // 핀란드, Santa Claus Office
  

        for(int i=0 ; i<panoramas.Length; i++) 
        {
            Lat = panoramas[i].lat;
            Lng = panoramas[i].lng;
            panoID = panoramas[i].panoid;
			impo=panoramas[i].impo;

            _rotation = new Vector3(Lat, -Lng, 0.0f);
            _translate = new Vector3(0, 0, -37.5f);

			if(impo==false)
			{
				GameObject child = Instantiate(NonImpObj, transform.position, Quaternion.identity) as GameObject;
				child.transform.parent = earth.transform;
				child.GetComponent<StreetviewPoint>().SetPosition(impo);
			}
			else
			{
				GameObject child2 = Instantiate(ImpObj, transform.position, Quaternion.identity) as GameObject;
				child2.transform.parent = earth.transform;
				child2.GetComponent<StreetviewPoint>().SetPosition(impo);
			}

        }
    }

    // Update is called once per frame
    void Update()
    {


    }

    
}
