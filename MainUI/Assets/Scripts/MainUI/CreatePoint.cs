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

public class CreatePoint : MonoBehaviour
{
    public GameObject obj;
    public GameObject earth;

    public PanoramaInfo[] panoramas = new PanoramaInfo[33];

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
		panoramas[0] = new PanoramaInfo(50.059139f, -122.958407f, "Zzl28rqGJgaL2IdkUleP8A"); // 캐나다, 브리티시컬럼비아주휘슬러-블랙콤 스키 리조트
		panoramas[1] = new PanoramaInfo(21.441916f, -157.808187f, "UPGaj__Xj9yHswn5Ul0yKg"); // 미국, Heeia State Park
		panoramas[2] = new PanoramaInfo(36.065096f, -112.137107f, "Fa-wHCWazJG6bn7ZjISQCA"); // 미국, Grand Canyon
		panoramas[3] = new PanoramaInfo(37.172210f, -93.322539f, "ayOK2SvpqYzlk4rm1Q4KPQ"); // 미국, Mizumoto Japanese Stroll Garden
		panoramas[4] = new PanoramaInfo(38.907877f, -77.072374f, "r_4YsN868xJX25PKfCjXZQ"); // 미국, Georgetown University
		panoramas[5] = new PanoramaInfo(-3.825792f, -32.396424f, "rfgb-QoM16gAAAQY_-2qjg"); // 미국, Catlin Seaview Survey
		panoramas[6] = new PanoramaInfo(-0.396520f, -90.292014f, "QY2gQ6TM6M3DECEX7Ie3xQ"); // 에콰도르, Galapagos
		panoramas[7] = new PanoramaInfo(-3.852043f, -32.442343f, "-dqeDyeJ_jVbfMxRF6jUJw"); // 브라질, Mirante
		panoramas[8] = new PanoramaInfo(-23.545225f, -46.474271f, "JUyCJZzn1kQ6iX9ClR4HYg"); // 브라질, Arena Corinthians
		panoramas[9] = new PanoramaInfo(68.509006f, 27.481845f, "GtnAyp0MCbYAAAQZLDcQIA"); // 핀란드, Northern Lights
		panoramas[10] = new PanoramaInfo(66.543346f, 25.848498f, "hEuDBjnLcagAAAQZA4Rr0g"); // 핀란드, Santa Claus Office
		panoramas[11] = new PanoramaInfo(53.569120f, -3.097381f, "PnH9mSC1eHs5U0jFoeuGRA"); // 영국, Formby
		panoramas[12] = new PanoramaInfo(42.545339f, 1.473427f, "me3DnHB6RmyGW8HEqAb5SQ"); // 안도라, Carrer Major
		panoramas[13] = new PanoramaInfo(44.315980f, 9.174589f, "p85y89ZBexcxofkwZpkHBQ"); // 이탈리아, Area marina protetta Portofino
		panoramas[14] = new PanoramaInfo(46.162368f, 10.945823f, "rUnaHHrABT2HTKastUtiDQ"); // 이탈리아, Dolomiti di Brenta
		panoramas[15] = new PanoramaInfo(45.432805f, 12.340583f, "CgDdLMNL25XX9atw3dEZog"); // 이탈리아, Canali di Venezia
		panoramas[16] = new PanoramaInfo(47.507172f, 19.041730f, "Pon_HbPw8a3TeuFFwfrYFA"); // 헝가리, Danube
		panoramas[17] = new PanoramaInfo(40.049812f, 26.219028f, "k4YxYzGcIOz0lKFkaOjBBw"); // 터키, Canakkale Martyrs' Memorial
		panoramas[18] = new PanoramaInfo(30.851549f, 29.664156f, "v5y4cLV08mWD7mA-BFkjoA"); // 이집트, Monastery of Saint Mina
		panoramas[19] = new PanoramaInfo(29.975257f, 31.138729f, "M9XnT_J1KWLORH4M-_v6sA"); // 이집트, Great Sphinx of Giza
		panoramas[20] = new PanoramaInfo(-33.799074f, 18.374987f, "cDSroLocAKL-Bgz_yvD4PQ"); // 남아프리카, Robben Island
		panoramas[21] = new PanoramaInfo(-20.250515f, 44.418992f, "88qwsWCYsJ8xXo2W1kjIXw"); // 마다가스카르, Baobab-Andronomea
		panoramas[22] = new PanoramaInfo(25.195232f, 55.276428f, "eznax7JvTOXrAztKUfqj-A"); // 아랍에미리트 연합, Burj Khalifa
		panoramas[23] = new PanoramaInfo(23.148599f, 53.735961f, "ZGBLHEQEe8-bc5E4tO8ekg"); // 아랍에미리트 연합, Liwa Desert
		panoramas[24] = new PanoramaInfo(18.922323f, 72.834289f, "1uYAeoWlY4Pqoa6PZBDyZA"); // 인도, Gateway Of India Mumbai
		panoramas[25] = new PanoramaInfo(25.307760f, 83.010823f, "PPZA0Nt_GDMAAAQfCWO9Tw"); // 인도, Man Singh Observatory
		panoramas[26] = new PanoramaInfo(27.931109f, 86.804662f, "TLc2AM_nU88AAAQpjCtyxw"); // 네팔, Everest Base Camp
		panoramas[27] = new PanoramaInfo(27.790253f, 86.718314f, "oL7LeZ1xLB-Xq9Qom5CWBQ"); // 네팔, Hillary Suspension Bridge
		panoramas[28] = new PanoramaInfo(13.441824f, 103.858065f, "LLLgCrnde_oJztsG3uzDcQ"); // 캄보디아, Bayon Temple
		panoramas[29] = new PanoramaInfo(37.571199f, 126.968461f, "zMrHSTO0GCYAAAQINlCkXg"); // 대한민국, Gyeonghui Palace
		panoramas[30] = new PanoramaInfo(33.459519f, 126.939750f, "IUmXlW6pRu1w9QnUdQk4vw"); // 대한민국, 성산일출봉
		panoramas[31] = new PanoramaInfo(34.395099f, 132.453466f, "0yx9g39tmnbtTUT1bZfyiw"); // 일본, 히로시마 평화기념관
		panoramas[32] = new PanoramaInfo(-33.579289f, 151.309294f, "HrHYIKEBk4Gqden0lMxJqA"); // 호주, Ku-ring-gai Chase National Park

        for(int i=0 ; i<panoramas.Length; i++) 
        {
            Lat = panoramas[i].lat;
            Lng = panoramas[i].lng;
            panoID = panoramas[i].panoid;

            _rotation = new Vector3(Lat, -Lng, 0.0f);
            _translate = new Vector3(0, 0, -37.5f);

            GameObject child = Instantiate(obj, transform.position, Quaternion.identity) as GameObject;

            child.transform.parent = earth.transform;
            child.GetComponent<StreetviewPoint>().SetPosition();
            
        }
    }

    // Update is called once per frame
    void Update()
    {


    }

    
}
