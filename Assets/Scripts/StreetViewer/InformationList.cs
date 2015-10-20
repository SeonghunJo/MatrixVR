using UnityEngine;
using System.Collections;

using System.Collections.Generic;

public class PlaceInfo
{
	public string country;		//이름
	public string area;      	//지역
	public string feature;   	//특징
	//public Texture flag;    	//국기	
	
	public PlaceInfo(string country, string area, string feature)
	{
		this.country = country;
		this.area = area;
		this.feature = feature;
	}
}

public class InformationList {

	public List<PlaceInfo> placelist;
	public InformationList()
	{
		placelist = new List<PlaceInfo>(); 
		Initialize ();
	}


	public void Initialize() {

		placelist.Add(new PlaceInfo("네팔", "사가르마타", "사가르마타에는 세계 최고봉(8,848m)인 에베레스트 산이 높이 솟아 있으며 그 외에도 여러 개의 높은 봉우리와 빙하, 깊은 계곡이 있다. 이곳에서는 눈 표범, 레서 판다와 같은 희귀종이 발견되었다. 독특한 문화를 갖고 있는 셰르파는 이 지역에 대한 흥미를 더해 준다."));
		placelist.Add(new PlaceInfo("네팔", "사가르마타", "사가르마타에는 세계 최고봉(8,848m)인 에베레스트 산이 높이 솟아 있으며 그 외에도 여러 개의 높은 봉우리와 빙하, 깊은 계곡이 있다. 이곳에서는 눈 표범, 레서 판다와 같은 희귀종이 발견되었다. 독특한 문화를 갖고 있는 셰르파는 이 지역에 대한 흥미를 더해 준다."));
		placelist.Add(new PlaceInfo("네팔", "사가르마타", "사가르마타에는 세계 최고봉(8,848m)인 에베레스트 산이 높이 솟아 있으며 그 외에도 여러 개의 높은 봉우리와 빙하, 깊은 계곡이 있다. 이곳에서는 눈 표범, 레서 판다와 같은 희귀종이 발견되었다. 독특한 문화를 갖고 있는 셰르파는 이 지역에 대한 흥미를 더해 준다."));
		placelist.Add(new PlaceInfo("네팔", "사가르마타", "사가르마타에는 세계 최고봉(8,848m)인 에베레스트 산이 높이 솟아 있으며 그 외에도 여러 개의 높은 봉우리와 빙하, 깊은 계곡이 있다. 이곳에서는 눈 표범, 레서 판다와 같은 희귀종이 발견되었다. 독특한 문화를 갖고 있는 셰르파는 이 지역에 대한 흥미를 더해 준다."));

	}
}
