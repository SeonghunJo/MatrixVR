using UnityEngine;
using System.Collections;

using System.Collections.Generic;

public class PanoramaInfo
{
    public float lat;       //위도
    public float lng;       //경도
    public string panoid;   //파노라마 ID

	public string title;
	public string country;
	public string area;
	public string contents;
	public string info_flag_path;

    public bool impo;       //추천 지역(T/F)

    public PanoramaInfo(float lat, float lng, string panoid,string title, 
	                    string country, string area, string contents,string info_flag_path, bool impo = true)
    {
        this.lat = lat;
        this.lng = lng;
        this.panoid = panoid;
        this.impo = impo; //  impo;

		this.title = title;
		this.country = country;
		this.area = area;
		this.contents = contents;
		this.info_flag_path = info_flag_path;
    }
}

public class CreateTarget : MonoBehaviour
{
    public GameObject impoObj;
    public GameObject nonImpobj;
    public GameObject earth;

    public List<PanoramaInfo> panoramas = new List<PanoramaInfo>();  

    //입력된 값을 float로 저장할 변수
    // Use this for initialization
    void Start()
    {
		// 네팔
		panoramas.Add(new PanoramaInfo(27.718398f, 86.716000f, "kFuSB9hD_6yJy8oxmjr1RQ", "Mudslide Bridge", "네팔", "사가르마타", "사가르마타에는 세계 최고봉(8,848?)인 에베레스트 산이 높이 솟아 있으며 그 외에도 여러 개의 높은 봉우리와 빙하, 깊은 계곡이 있다. 이곳에서는 눈 표범, 레서 판다와 같은 희귀종이 발견되었다. 독특한 문화를 갖고 있는 셰르파는 이 지역에 대한 흥미를 더해 준다."
		                               ,"1네팔", false)); //Mudslide Bridge

		// 대만
		panoramas.Add(new PanoramaInfo(25.036556f, 121.517962f, "uM-jW1fT93Y_eJ7CPTG6jA", "장개석 기념관", "대만", "타이베이", "타이베이 시 중정구에 위치해 있는 장개석 기념관(중정기념당)은 중화민국의 초대 총통이었던 장개석을 기념해 1980년에 건설한 기념관이다."
		                               ,"2대만", false)); //장개석 기념관
		
		// 대한민국
		panoramas.Add(new PanoramaInfo(37.571199f, 126.968461f, "zMrHSTO0GCYAAAQINlCkXg", "경희궁", "대한민국", "서울특별시 종로구", "경희궁은 서울시에 있는 조선 시대 궁궐로 광해군 10년(1623년)에 건립한 이후, 10대에 걸쳐 임금이 정사를 보았던 궁궐이다. 경복궁, 창경궁과 함께 조선왕조의 3대궁으로 꼽힐 만큼 큰 궁궐이었으나 일제강점기에 심하게 훼손되어 현재 남아있는 건물은 정문이었던 흥화문과 정전이었던 숭정전, 그리고 후원의 정자였던 황학정까지 세 채에 불과하다. 그나마 초석과 기단이 남아 있고, 뒤쪽에는 울창한 수림이 잘 보전돼 있어 궁궐의 자취를 잘 간직하고 있는 편이다."
		                               ,"3대한민국", false)); //Gyeonghui Palace
		panoramas.Add(new PanoramaInfo(33.459519f, 126.939750f, "IUmXlW6pRu1w9QnUdQk4vw", "성산일출봉", "대한민국", "제주 서귀포시", "성산 일출봉은 서귀포시 성산읍에 있는 산이다. 분화구 높이는 182m 이며, 성산 일출봉에서의 일출은 영주십경 중 하나이다. 일출봉 분화구와 주변 1km 해역은 성산 일출봉 천연보호구역으로 대한민국의 천연기념물로 지정되어 있다."
		                               ,"4대한민국", false)); //성산일출봉
		
		// 미국
		panoramas.Add(new PanoramaInfo(37.172210f, -93.322539f, "ayOK2SvpqYzlk4rm1Q4KPQ", "Mizumoto Japanese Stroll Garden", "미국", "캘리포니아", "일본식 정원을 재현해 놓은 재패니즈 가든. 황제와 귀족의 정원은 레크레이션 및 미적 쾌락을 위해 설계되었으나 종교적인 정원은 사색과 명상을 위해 설계되었다."
		                               ,"5미국", true)); //Mizumoto Japanese Stroll Garden
		panoramas.Add(new PanoramaInfo(-3.825792f, -32.396424f, "rfgb-QoM16gAAAQY_-2qjg", "Catlin Seaview Survey", "미국", "미국", "해경 조사 지역으로 과학적 탐구를 위한 수많은 산호초 연구가 이루어지고 있고, 많은 종류의 물고기와 돌고래를 관측할 수 있다."
		                               ,"6미국", true)); //돌고래 Catlin Seaview Survey
		panoramas.Add(new PanoramaInfo(40.688619f, -74.044125f, "cthAMoR7m9cmP-wUC8AIWA", "자유의 여신상", "미국", "뉴욕", "미국과 프랑스 국민들 간의 친목을 기념하고, 미국의 독립 100주년을 기념하기 위해 프랑스인들의 모금운동으로 증정되었으며, 1886년에 완공되었다. 미국의 자유와 민주주의의 상징이고, 1984년에는 유네스코 세계문화유산에 등록되었다."
		                               ,"7미국", true)); //Statue of Liberty National Monument
		panoramas.Add(new PanoramaInfo(-14.865306f, 145.680527f, "nNw59vW3owAAAAQfCaaiYQ", "Minke Whales", "미국", "미국", "일반적으로 대형 긴수염고래류와는 형태적으로 쉽게 구별된다. 머리는 옆이나 위에서 보아도 뾰족하게 나있다. 등지느러미는 높고, 뒤로 굽혀있으며 몸길이의 2/3 정도에 위치해 있다. 몸 색은 특징적으로 등은 검은색이 나는 회색이고 배는 흰색이다. 가슴지느러미 중앙을 가로지르는 흰 문양이 특징적이나 서식해역에 따라 차이가 있다."
		                               ,"8미국", true)); //Minke Whales
		
		// 브라질
		panoramas.Add(new PanoramaInfo(-3.137761f, -60.493355f, "1ci-8iBT_UuG1dlrUy1vzg", "Rio Negro", "브라질", "브라질 북서부", "네그루 강은 아마존 강 북부의 지류 중 가장 크고, 검은색을 띤 강으로는 세계 최대의 길이와 유량을 가지고 있다. 하천의 이름 네그루란 ‘검다’는 뜻으로, 식물의 잔해가 완전히 분해되지 못한 상태에서 발생하는 유기산 때문이다. 검은색 강과 흰색 강이 만나 섞이지 않고 흐르는 현상, 물속에서 자라는 숲 이가포 등 네그루 강의 독특한 생태계를 경험하기 위해 많은 사람들이 찾고 있다."
		                               ,"9브라질" , true)); //Rio Negro 아마존
		
		// 아랍에미리트 연합
		panoramas.Add(new PanoramaInfo(25.195232f, 55.276428f, "eznax7JvTOXrAztKUfqj-A", "Burj Khalifa", "아랍에미리트 연합", "두바이", "부르즈 할리파는 아랍에미리트 두바이의 신도심 지역에 있는 높이 829.8미터의 초고층 건물이다. 완공 이전 이름은 부르즈 두바이로, 아랍에미리트의 대통령인 할리파 빈 자이드 알나하얀의 이름을 본따 부르즈 두바이에서 부르즈 할리파로 개명되었다. 현재까지 완성된 초고층 건물 중에서 가장 높아 지상층에서 최고층까지 초고속 엘리베이터로 약 1분이 걸리며, 세계에서 가장 높은 인공 구조물이다."
		                               ,"10아랍", true)); //Burj Khalifa
		
		// 영국
		panoramas.Add(new PanoramaInfo(51.178946f, -1.826564f, "VxzhBNNu-VGQC8HtVIaY3A", "스톤헨지", "영국", "윌트셔주 솔즈베리 평원", "스톤헨지는 선사 시대의 거석기념물에 있는 원형으로 배치된 유적이다. 높이 8미터, 무게 50톤인 거대 석상 80여 개가 세워져 있다. 수수께끼의 선사시대 유적으로 누가, 어떻게, 왜 만들었는가에 대한 의문이 풀려지지 않고 있다."
		                               ,"11", true)); //스톤헨지
		
		// 이탈리아
		panoramas.Add(new PanoramaInfo(46.162368f, 10.917272f, "5_AWZhbCGuqMd0AGfOlOUw", "동오미티 벨루네시 국립공원", "이탈리아", "베네토주 벨루노", "수천 년 전부터 사람이 살아온 흔적이 발견되었는데, 그중 발레 임페리나의 탄광 중심지에 있는 몇 곳의 선사시대 고고학 유적이 중요하다. 그밖에 카르투지오 수도회의 수도원, 산록지대의 작은 교회들, 중세시대의 여행자 숙박소, 제1차 세계대전 당시의 군사도로, 양치기 헛간 등 중세를 거쳐 현대에 이르는 다양한 유적이 남아 있다. 현재 유네스코의 세계문화유산으로 지정되어있다."
		                               ,"12이탈리아", true)); // Dolomiti UNESCO2
		panoramas.Add(new PanoramaInfo(41.890072f, 12.492534f, "07gbqMWIg_HId5m7W94qHg", "콜로세움", "이탈리아", "로마", "콜로세움은 고대 로마 시대의 건축물 가운데 하나로 로마 제국 시대에 만들어진 원형 경기장이다. 현재는 로마를 대표하는 유명한 관광지로 탈바꿈하였다. 콜로세움이라는 이름은 근처에 있었던 네로 황제의 동상에서 유래한다. 원래 이름은 플라비우스 원형 극장이었다."
		                               ,"13이탈리아", false)); //로마 콜로세움
		
		// 인도
		panoramas.Add(new PanoramaInfo(15.949466f, 75.815568f, "oG_6nL1zn2L5SiHB7fEqBQ", "Pattadakal", "인도", "남부 카르나타카주", "파타다칼은 인도 카르나타카 주 북부에 위치한 마을로, 8세기에 건설된 힌두교 사원 유적으로 유명한 곳이다. 1987년 유네스코가 지정한 세계유산으로 선정되었으며 북부의 드라비다인 건축 양식과 남부의 아리아인 건축 양식이 혼합된 것이 특징이다."
		                               ,"14인도", false)); //Group of Mounments at Pattadakal
		panoramas.Add(new PanoramaInfo(19.900922f, 75.320179f, "PxmiVxloPR5XpPOC3sDb3A", "Bibi Ka Maqbara", "인도", "아우랑가바드", "비비 까 마끄바라(여자의 무덤)은 무굴황제 아우랑제브의 첫 번째 부인인 라비아 웃 다우라니의 무덤으로 큰 아들인 아잠 샤에 의해 건설되었다. 아잠 샤의 친할머니 뭄 따즈마할의 묘, 타지마할을 본 따 만들어 졌으며 작은 타지마할로 불린다."
		                               ,"15인도" , true)); //Bibi Ka Maqbara
		
		// 인도네시아
		panoramas.Add(new PanoramaInfo(-8.737039f, 119.412259f, "Ri71LYeNvGsAAAQW4U3bQQ", "코모도섬", "인도네시아", "누사텡가라티무르 주", "발리에서 동쪽으로 483킬로미터 떨어진 플로레스 섬과 숨바와 섬 사이에 있는 코모도 섬은 코모도왕도마뱀 2,500여 마리의 서식지이다. 코모도 섬은 자바, 발리, 수마트라 같은 큰 섬과 파드라와 린카 같은 작은 섬들을 만든 거대한 화산작용으로 생성되었다. 이 섬은 1980년에는 국립공원으로, 1992년에는 세계유산으로 지정되었다."
		                               ,"16인도네시아" , true)); //Komodo Island
		
		// 중국
		panoramas.Add(new PanoramaInfo(34.557692f, 117.742510f, "rY5ZW6S3sFUAAAAGOzOdpg", "타이얼좡 고성", "중국", "산둥성", "중국의 남방과 북방이 만나는 접점지대의 옛 마을인 조장의 타이얼좡. 중국의 남방문화와 북방문화 등 다양한 지역 문화를 볼 수 있다. 폐허가 된 고성을 2008년에 중건하기로 선포하고 공사기간을 거쳐 2010년 5월부터 정식으로 여행객을 받게 되었다."
		                               ,"17중국", true)); //Datamen Street
		
		// 프랑스
		panoramas.Add(new PanoramaInfo(48.861780f, 2.288130f, "psUz8wNiVzzOSuElXeSG0g", "에펠탑", "프랑스", "파리", "에펠탑은 1889년 파리 마르스 광장에 지어진 탑이다. 프랑스의 대표 건축물인 이 탑은 격자 구조로 이루어져 파리에서 가장 높은 건축물이며, 매년 수백만 명이 방문할 만큼 세계적인 유료 관람지이다. 이를 디자인한 귀스타브 에펠의 이름에서 명칭을 얻었으며, 1889년 프랑스 혁명 100주년 기념 세계 박람회의 출입 관문으로 건축되었다."
		                               ,"18프랑스", true)); //에펠탑
		
		// 핀란드
		panoramas.Add(new PanoramaInfo(68.509006f, 27.481845f, "GtnAyp0MCbYAAAQZLDcQIA", "북극광", "핀란드", "라피주", "오로라라고도 불리는 북극광은 남반구의 오스트레일리아 오로라와 함께 자연이 우리에게 보내는 가장 아름다운 선물 중 하나이다. 매년 북극 라플란드의 하늘에는 신비로운 오로라가 펼쳐지는데 그 횟수는 200회에 달한다. 이 현상은 1초에 1,000킬로미터의 속도로 확산되는 플라스마의 흐름인 태양풍과 지구 자기장 사이에 일어나는 상호작용의 결과이다."
		                               ,"19핀란드", true)); //Northern Lights
		
		// 호주
		panoramas.Add(new PanoramaInfo(21.479702f, -86.632599f, "xq5H2tvkw_AAAAQJLmg-7A", "고래 상어", "호주", "호주", "고래상어는 가장 큰 어류로 길이가 15m, 무게 40톤에 달한다. 넓이 1.5m에 달하는 큰 입과 작은 눈, 흰색의 줄무늬와 점무늬가 특징이다. 어류인데도 포유류인 고래 이름을 붙여 고래상어로 불리는 것은 덩치가 고래만큼 크고 먹이 사냥 방식 또한 수염고래를 닮았기 때문이다."
		                               ,"20호주", true)); //Whale Sharks

       


		/*
        // 남극
        panoramas.Add(new PanoramaInfo(-62.596087f, -59.901655f, "ZzuMubmHCfCGGo3ePSlpCQ", true)); // 펭귄 서식지

        // 네덜란드
        panoramas.Add(new PanoramaInfo(51.881662f, 4.643277f, "e1H3GgZkFLIvm2UcV0AZeA", false)); //엘슈트 풍차망
        panoramas.Add(new PanoramaInfo(52.113498f, 4.280813f, "So0MXeaPWZXH0Dp1r-Z_Tw", false)); //스헤베닝겐

        // 네팔
        panoramas.Add(new PanoramaInfo(27.931109f, 86.804662f, "TLc2AM_nU88AAAQpjCtyxw", false)); //Everest Base Camp
        panoramas.Add(new PanoramaInfo(27.790246f, 86.718457f, "Cypswt1Mnvde-rgEl5jMUQ", true)); //Hillary Suspension Bridge
        panoramas.Add(new PanoramaInfo(27.994511f, 86.828381f, "_JmXNYUY9qAZHxIyYUWhRQ", true)); //Kala Pattar
        panoramas.Add(new PanoramaInfo(27.718398f, 86.716000f, "kFuSB9hD_6yJy8oxmjr1RQ", false)); //Mudslide Bridge
        panoramas.Add(new PanoramaInfo(27.926364f, 86.796615f, "YJCqovBj5_EKjvQ_9tSvQQ", false)); //Taboche Mountain

        // 노르웨이
        panoramas.Add(new PanoramaInfo(59.857361f, 6.358727f, "ZxCWoCUti1cbv7_pMLzBhg", false)); //피아에라

        // 대만
        panoramas.Add(new PanoramaInfo(25.038821f, 121.560306f, "htCkJktfaeM2DvPEWyg3-A", false)); //선얏센 기념관
        panoramas.Add(new PanoramaInfo(25.036556f, 121.517962f, "uM-jW1fT93Y_eJ7CPTG6jA", false)); //장개석 기념관

        // 대한민국
        panoramas.Add(new PanoramaInfo(37.571199f, 126.968461f, "zMrHSTO0GCYAAAQINlCkXg", false)); //Gyeonghui Palace
        panoramas.Add(new PanoramaInfo(33.459519f, 126.939750f, "IUmXlW6pRu1w9QnUdQk4vw", false)); //성산일출봉
        panoramas.Add(new PanoramaInfo(33.527656f, 126.769749f, "qrvXxrP3D4oAAAQWtNh2dQ", true)); //만장굴

        // 러시아
        panoramas.Add(new PanoramaInfo(56.309959f, 38.129472f, "GrFw-S4a5A6xdQWs9FhEhw", true)); //The Holy Trinity-St.Sergius Lavra
        panoramas.Add(new PanoramaInfo(43.354961f, 42.439195f, "A5oCY2VD27HtxwTgBfI7Jw", false)); //West Summit

        // 마다가스카르
        panoramas.Add(new PanoramaInfo(-20.250515f, 44.418992f, "88qwsWCYsJ8xXo2W1kjIXw", false)); //Baobab-Andronomea
        panoramas.Add(new PanoramaInfo(-23.541324f, 43.779575f, "6R4EPTccvk941C4fobch5g", false)); //St. Augustin
        panoramas.Add(new PanoramaInfo(-18.923246f, 47.532037f, "mYYUd4xWYQYgepHeFmYWxA", false)); //Antananarivo
        panoramas.Add(new PanoramaInfo(-18.566859f, 43.887191f, "Eu_xDQX5IZPRJKK3L3zafA", false)); //Nosy Manghily
        panoramas.Add(new PanoramaInfo(-13.480002f, 48.326310f, "0NKx1dxm9zHKuh3QLJkddQ", false)); //Nosy Komba

        // 멕시코
        panoramas.Add(new PanoramaInfo(19.434387f, -99.141486f, "AS1HPRlUroiellKWwJ_N8w", false)); //예술의 궁전

        // 모나코
        panoramas.Add(new PanoramaInfo(43.730309f, 7.426062f, "b6XBf_pbLLQAAAQJKij6qQ", false)); //Roche Saint Nicholas

        // 미국
        panoramas.Add(new PanoramaInfo(38.625576f, -90.188635f, "fFRuX5QWUEISQWxwl8HGuw", false)); //게이트웨이 아치
        panoramas.Add(new PanoramaInfo(36.065096f, -112.137107f, "Fa-wHCWazJG6bn7ZjISQCA", true)); //Grand Canyon
        panoramas.Add(new PanoramaInfo(37.172210f, -93.322539f, "ayOK2SvpqYzlk4rm1Q4KPQ", true)); //Mizumoto Japanese Stroll Garden
        panoramas.Add(new PanoramaInfo(-3.825792f, -32.396424f, "rfgb-QoM16gAAAQY_-2qjg", true)); //돌고래 Catlin Seaview Survey
        panoramas.Add(new PanoramaInfo(40.688619f, -74.044125f, "cthAMoR7m9cmP-wUC8AIWA", true)); //Statue of Liberty National Monument
        panoramas.Add(new PanoramaInfo(34.236399f, -77.953794f, "l0eKEYaRXbn6csDEH_bhPw", false)); //Battleship NORTH CAROLINA
        panoramas.Add(new PanoramaInfo(35.071705f, -82.886928f, "gaeVY57miaQwXI2gBlIbcg", false)); //Jocassee Gorges
        panoramas.Add(new PanoramaInfo(21.269602f, -157.695598f, "_HKq3kJG3NNXV9ElI9J8Wg", false)); //Hanauma Bay
        panoramas.Add(new PanoramaInfo(21.277735f, -157.833614f, "27zsgCoYo3GFeBSLD3IXfw", false)); //Waikiki Beach
        panoramas.Add(new PanoramaInfo(65.853701f, 51.403202f, "1R1EqRq4A_vnGToVxvCV-A", true)); //Byodo-In Temple
        panoramas.Add(new PanoramaInfo(35.031650f, -111.026837f, "S2IQmPwHGhJ-YCXugFkM-Q", false)); //Meteor Crater
        panoramas.Add(new PanoramaInfo(21.675655f, -158.039678f, "I3e6j5rY_OEMj_G8sjnxOQ", false)); //Sunset Beach
        panoramas.Add(new PanoramaInfo(33.127904f, -117.311509f, "MSY7skkC8g5A5L2gDwWwQg", false)); //Lego Land
        panoramas.Add(new PanoramaInfo(28.474642f, -81.466547f, "sglA5AcgqpG0sQ9o1gDOSg", false)); //Universal Studios
        panoramas.Add(new PanoramaInfo(21.306635f, -157.859008f, "C5J9QXgPhZo-LnZvZ7FWCg", false)); //Iolani Palace
        panoramas.Add(new PanoramaInfo(20.634702f, -156.497456f, "FUMjYMh38n_EitPWdt70RA", false)); //Maui, Hawaii, Molokini Crater
        panoramas.Add(new PanoramaInfo(20.311091f, -87.036529f, "691D23S_viUAAAQJKigqjA", false)); //Columbia Deep
        panoramas.Add(new PanoramaInfo(21.479702f, -86.632599f, "xq5H2tvkw_AAAAQJLmg-7A", true)); //Whale Sharks
        panoramas.Add(new PanoramaInfo(19.419751f, -155.288205f, "NsXMzG6eODgN7UjSs9G2nA", false)); //Hawaii Volcanoes National Park
        panoramas.Add(new PanoramaInfo(19.927064f, -155.886999f, "XTcMO38szyVGzYIDvpyIzQ", false)); //Hilton Waikoloa Village
        panoramas.Add(new PanoramaInfo(47.620012f, -122.349345f, "TlSB0Ge9RrEPPI-oln365A", false)); //Space Needle
        panoramas.Add(new PanoramaInfo(43.877169f, -103.456022f, "5c1XDr-37d5xdtmWQ79LDw", true)); //Mount Rushmore National Memorial
        panoramas.Add(new PanoramaInfo(46.552484f, -86.459171f, "hodDbHroQu5nFeLgwWxtlw", false)); //Pictured Rocks National Lakeshore
        panoramas.Add(new PanoramaInfo(-33.956262f, 151.265231f, "4p_ncfuaBCcAAAQfCZU9KA", true)); //Sydney Magic Point Sharks
        panoramas.Add(new PanoramaInfo(-33.802112f, 151.302226f, "GFwh75SkDaEAAAQfCZXXmw", true)); //Sydney Outside Manly
        panoramas.Add(new PanoramaInfo(-18.246795f, 147.386806f, "hvJbgfc-HtoAAAQfCaR8Mw", false)); //Myrmidon Reef
        panoramas.Add(new PanoramaInfo(-16.427967f, 145.989965f, "U5OACYt--5UAAAQfCbAC4A", true)); //Norman Reef
        panoramas.Add(new PanoramaInfo(-14.865306f, 145.680527f, "nNw59vW3owAAAAQfCaaiYQ", true)); //Minke Whales
        panoramas.Add(new PanoramaInfo(-13.921229f, 144.641367f, "MGB3IbSxGxkAAAQfCa_m3w", false)); //North Broken Passage

        // 벨리즈
        panoramas.Add(new PanoramaInfo(17.204902f, -87.544919f, "ByV4feOfB24AAAQWx3MuEw", false)); //Half Moon Caye

        // 브라질
        panoramas.Add(new PanoramaInfo(-3.137761f, -60.493355f, "1ci-8iBT_UuG1dlrUy1vzg", true)); //Rio Negro 아마존

        // 스페인
        panoramas.Add(new PanoramaInfo(41.404212f, 2.174820f, "lXSocJr_ApZrMdq_CrfUNg", false)); //사그라다 파밀리아
        panoramas.Add(new PanoramaInfo(40.961402f, -5.666230f, "4UvBd8mYTnnAnQLOLG_AOw", true)); //쿠엥카 구 성곽도시
        panoramas.Add(new PanoramaInfo(40.948054f, -4.116914f, "XQN5MyfKSxK9qNT1TQleJw", true)); //세고비아 구 시가지
        panoramas.Add(new PanoramaInfo(40.419397f, -3.693281f, "1Hdng3Mj9y_OU-37-vnpkg", true)); //시벨레스 광장

        // 아랍에미리트 연합
        panoramas.Add(new PanoramaInfo(25.195232f, 55.276428f, "eznax7JvTOXrAztKUfqj-A", true)); //Burj Khalifa
        panoramas.Add(new PanoramaInfo(23.148599f, 53.735961f, "ZGBLHEQEe8-bc5E4tO8ekg", false)); //Liwa Desert

        // 아르헨티나
        panoramas.Add(new PanoramaInfo(-32.653135f, -70.012024f, "W9nMFNrPp8GrTDYyVaHYBg", true)); //Aconcagua Summit

        // 안도라
        panoramas.Add(new PanoramaInfo(42.545339f, 1.473427f, "me3DnHB6RmyGW8HEqAb5SQ", false)); //Carrer Major

        // 영국
        panoramas.Add(new PanoramaInfo(57.145373f, -4.672888f, "QHdANWuiOm808C1fvSBVKA", false)); //Loch Ness
        panoramas.Add(new PanoramaInfo(52.201062f, -2.458700f, "M4Nd42bBHLn7XLm3SR31Iw", true)); //Brockhampton Estate
        panoramas.Add(new PanoramaInfo(52.457104f, -0.553142f, "Nyg5vDL24yfSxm3NqcL0sg", true)); //Lyveden New Bield
        panoramas.Add(new PanoramaInfo(51.500891f, -0.122697f, "F0cE4sooBkl9S7ERvTvwIQ", false)); //빅벤
        panoramas.Add(new PanoramaInfo(51.506607f, -0.074633f, "4cuk-4G-vKM63svR1rsFHw", false)); //타워 브리지
        panoramas.Add(new PanoramaInfo(51.178946f, -1.826564f, "VxzhBNNu-VGQC8HtVIaY3A", true)); //스톤헨지
        panoramas.Add(new PanoramaInfo(50.116583f, -5.478983f, "b56yzD7fyUBnqVVxFDivBQ", false)); //St Michael's Mount
        panoramas.Add(new PanoramaInfo(50.688888f, -1.956280f, "PbGl6pMgny7wahDD7z8AtQ", false)); //Brownsea island

        // 우크라이나
        panoramas.Add(new PanoramaInfo(48.522029f, 26.498246f, "ErnD9tZimEkkZ6VliXUphw", false)); //Chotin Castle
        panoramas.Add(new PanoramaInfo(48.296993f, 25.924853f, "ydVs_ymCLcW5hmiKpHHqyw", false)); //Chernivtsi State University

        // 이집트
        panoramas.Add(new PanoramaInfo(29.975257f, 31.138729f, "M9XnT_J1KWLORH4M-_v6sA", false)); //Great Sphinx of Giza
        panoramas.Add(new PanoramaInfo(29.978089f, 31.135899f, "-B-Q0pE9LLgmDpLK38lWCQ", true)); //Khufu Ship, Pyramids

        // 이탈리아
        panoramas.Add(new PanoramaInfo(44.315980f, 6.174589f, "p85y89ZBexcxofkwZpkHBQ", false)); //Area marina protetta Portofino
        panoramas.Add(new PanoramaInfo(45.432005f, 12.328098f, "XKoFzFD4LeEyJozHuqTpzg", true)); //Canali di Venezia

        panoramas.Add(new PanoramaInfo(38.626372f, 15.063730f, "n9zp8zDXdpDUCDVlLRXOUg", false)); //Beaches of Panarea
        panoramas.Add(new PanoramaInfo(40.419843f, 15.005841f, "QQaZ4ikxtC4amp8BfxNM8g", false)); //Scavi di Paestum
        panoramas.Add(new PanoramaInfo(41.074990f, 14.327291f, "leUKewEsRlRNcA5UbHlIyQ", false)); //카세트라 왕궁
        panoramas.Add(new PanoramaInfo(41.890072f, 12.492534f, "07gbqMWIg_HId5m7W94qHg", false)); //로마 콜로세움
        panoramas.Add(new PanoramaInfo(43.318912f, 11.331626f, "ONVDa4I8_S4suzYciZQajA", false)); //시에나 역사지구
        panoramas.Add(new PanoramaInfo(45.464300f, 7.189493f, "MPsvZouZ8u_7Mds8mb39QA", true)); //Milano City Center

        panoramas.Add(new PanoramaInfo(45.813453f, 10.793751f, "GCV_ShhIYj0yp5QTFSnaPQ", false)); //Limone del Garda
        panoramas.Add(new PanoramaInfo(46.162368f, 10.917272f, "5_AWZhbCGuqMd0AGfOlOUw", true)); // Dolomiti UNESCO2

        // 인도
        panoramas.Add(new PanoramaInfo(18.922323f, 72.834289f, "1uYAeoWlY4Pqoa6PZBDyZA", false)); //Gateway Of India Mumbai
        panoramas.Add(new PanoramaInfo(12.305133f, 76.656141f, "ZjGqKKJpM8ofsZrw_CpAbA", false)); //Mysore Palace
        panoramas.Add(new PanoramaInfo(10.783169f, 79.131424f, "9RWy9wBv0b7hJVk74zmrYw", false)); //Thanjavur Temple
        panoramas.Add(new PanoramaInfo(15.949466f, 75.815568f, "oG_6nL1zn2L5SiHB7fEqBQ", false)); //Group of Mounments at Pattadakal
        panoramas.Add(new PanoramaInfo(19.887609f, 86.095086f, "ZBH5-GhJzmP0zuQywSdVqw", false)); //Sun Temple
        panoramas.Add(new PanoramaInfo(19.152107f, 77.317845f, "XhfJM9dq4mfrlnyX1i354g", false)); //Nanded Gurudwara
        panoramas.Add(new PanoramaInfo(20.551901f, 75.699198f, "j7W7UotIEokAAAQWjKM7kw", true)); //Ajanta Caves
        panoramas.Add(new PanoramaInfo(19.900922f, 75.320179f, "PxmiVxloPR5XpPOC3sDb3A", true)); //Bibi Ka Maqbara
        panoramas.Add(new PanoramaInfo(23.479370f, 77.740076f, "s4zigh3dsnyAYGq4jJw2-w", false)); //Sanchi Buddhist Stupas
        panoramas.Add(new PanoramaInfo(23.858889f, 72.101782f, "KmSe7jASKiBY1Rqx2aCzxg", false)); //Rani Ki Vav
        panoramas.Add(new PanoramaInfo(26.280984f, 73.047820f, "GvNKJetyrNX5zhro7l66mw", true)); //Umaid Bhawan Palace
        panoramas.Add(new PanoramaInfo(27.176266f, 78.021921f, "R0mRqgzFkr3ck05Kgp0BBA", false)); //Agra Fort
        panoramas.Add(new PanoramaInfo(25.749930f, 82.688613f, "g6UlLzOBanpcMVVQyZp8gQ", false)); //Jaunpur Fort

        // 인도네시아
        panoramas.Add(new PanoramaInfo(-8.737039f, 119.412259f, "Ri71LYeNvGsAAAQW4U3bQQ", true)); //Komodo Island

        // 일본
        panoramas.Add(new PanoramaInfo(35.658581f, 139.746281f, "LtUmBSKdGOjeeLlhby9dVA", false)); //도쿄 타워
        panoramas.Add(new PanoramaInfo(34.295907f, 132.318611f, "kEhefMsEl_j3fSb3YeJ-Jw", false)); //이쓰쿠시마 신사
        panoramas.Add(new PanoramaInfo(35.012762f, 135.750338f, "QARIAkA8fLMIvXlaG9BKRg", false)); //니조성
        panoramas.Add(new PanoramaInfo(35.015895f, 135.674029f, "owRdGfCtDxJqJSLOtMUOtw", false)); //텐류지
        panoramas.Add(new PanoramaInfo(32.806165f, 130.706273f, "5gf4IL6Vpsx5GdU9M3Hufg", false)); //구마모토 성

        // 중국
        panoramas.Add(new PanoramaInfo(34.557692f, 117.742510f, "rY5ZW6S3sFUAAAAGOzOdpg", true)); //Datamen Street

        // 체코
        panoramas.Add(new PanoramaInfo(50.088037f, 14.421055f, "YO6UbAb0JdnOJYF2MODFXA", false)); //프라하 역사지구
        panoramas.Add(new PanoramaInfo(48.810779f, 14.315088f, "l_SHzsaFFzpnRCVJv6OFAQ", false)); //체스키 그룸로프 역사지구
        panoramas.Add(new PanoramaInfo(49.593827f, 17.250690f, "qYEGZ8uWn_pI2lUseQDyeg", true)); //올로모우츠의 성삼위일체 석주

        // 캄보디아
        panoramas.Add(new PanoramaInfo(13.441824f, 103.858065f, "LLLgCrnde_oJztsG3uzDcQ", true)); //Bayon Temple
        panoramas.Add(new PanoramaInfo(13.474576f, 104.229077f, "Rsfd__NlKE0c-_3rVQW4fA", false)); //Beng Mealea Temple

        // 캐나다
        panoramas.Add(new PanoramaInfo(50.059139f, -122.958407f, "Zzl28rqGJgaL2IdkUleP8A", false)); //브리티시컬럼비아주휘슬러-블랙콤 스키 리조트
        panoramas.Add(new PanoramaInfo(43.081493f, -79.078126f, "42SshA7tZ3JfqnY3_zu-zw", true)); //Niagara Falls
        panoramas.Add(new PanoramaInfo(43.642097f, -79.385320f, "tNLct3NZIOciHZqwg2qKRg", false)); //CN Tower

        // 크로아티아
        panoramas.Add(new PanoramaInfo(45.823234f, 16.019663f, "isdebLyDj-h2SPWtE0rVFQ", false)); //막시미르 공원

        // 탄자니아
        panoramas.Add(new PanoramaInfo(-3.075645f, 37.352457f, "MXClB4d78t6qwBZDSAibPg", false)); //Kilimanjaro

        // 터키
        panoramas.Add(new PanoramaInfo(40.049812f, 26.219028f, "k4YxYzGcIOz0lKFkaOjBBw", false)); //Canakkale Martyrs' Memorial

        // 포르투갈
        panoramas.Add(new PanoramaInfo(41.137596f, -8.609771f, "thVBWkKMGg08eXF_uTYbQg", false)); //도우루강

        // 프랑스
        panoramas.Add(new PanoramaInfo(49.805015f, 4.119393f, "1GWafSZa22C6hW0j1L7a8A", false)); //베르사이유 궁전 ORIGIN : 48.805015f, 2.119393f
        panoramas.Add(new PanoramaInfo(49.856306f, 0.297560f, "PRDbHicwARPmhyIq9uKeCA", true)); //에펠탑 ORIGIN : 48.856306f, 2.297560f
        panoramas.Add(new PanoramaInfo(47.853663f, 2.347951f, "pQDLtpAbzF9KG4biw7zW4w", false)); //노트르담 대성당 ORIGIN : 48.853663f, 2.347951f
        panoramas.Add(new PanoramaInfo(45.873502f, 4.295779f, "Ixp4bKuPoKGyOBtKBUHVdg", true)); //개선문 ORIGIN : 48.873502f, 2.295779f
        panoramas.Add(new PanoramaInfo(45.861489f, 0.333644f, "cMMNd74PdSBvjw3vf9PByA", true)); //루브르 박물관 ORIGIN : 48.861489f, 2.333644f

        // 핀란드
        panoramas.Add(new PanoramaInfo(68.509006f, 27.481845f, "GtnAyp0MCbYAAAQZLDcQIA", true)); //Northern Lights

        // 필리핀
        panoramas.Add(new PanoramaInfo(9.085340f, 123.272577f, "nB4D43juZU6LYGYb0VIDig", false)); //Apo Island
        panoramas.Add(new PanoramaInfo(8.936190f, 120.008698f, "J67g8IADDUUAAAQWx3Mt6g", false)); //Tubbataha

        // 헝가리
        panoramas.Add(new PanoramaInfo(47.507172f, 19.041730f, "Pon_HbPw8a3TeuFFwfrYFA", false)); //Danube

        // 호주
        panoramas.Add(new PanoramaInfo(-33.579289f, 151.309294f, "HrHYIKEBk4Gqden0lMxJqA", false)); //Ku-ring-gai Chase National Park
        panoramas.Add(new PanoramaInfo(-34.137255f, 151.117934f, "_fA2Asn1hvsgqOJiJGDcTQ", false)); //Royal National Park
        panoramas.Add(new PanoramaInfo(-28.638596f, 153.636581f, "1MWcxwH8sjiGWmyufsczzA", false)); //Cape Byron State Conservation Area
        panoramas.Add(new PanoramaInfo(-23.303391f, 151.914955f, "TNE1NPAnEUNYhGk9kFWyJw", true)); //Wilson Island underwater
        panoramas.Add(new PanoramaInfo(-23.442896f, 151.906584f, "CWskcsTEZBNXaD8gG-zATA", true)); //Heron Island

        panoramas.Add(new PanoramaInfo(21.479702f, -86.632599f, "xq5H2tvkw_AAAAQJLmg-7A", true)); //Whale Sharks

		*/
        for (int i = 0; i < panoramas.Count; i++)
        {           
            
			PanoramaInfo info = panoramas[i];	

			//추천지역은 타겟의 모양을 다르게 			
			GameObject child = Instantiate( info.impo ? impoObj : nonImpobj , transform.position, Quaternion.identity) as GameObject;
			child.transform.parent = earth.transform;
			child.GetComponent<StreetViewPoint>().SetPosition(info); // 파라미터 2개 추가
			
			if ( Manager.Instance.enableAutoGathering
			    && !Utility.FindCachedImageFromID(info.panoid)
			    && info.impo ) {
				
				Manager.Instance.important = info.impo;
				Manager.Instance.panoramaID = info.panoid;
				Application.LoadLevel("StreetViewer");
			}

        }
    }

    // Update is called once per frame
    void Update()
    {


    }


}
