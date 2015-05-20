using UnityEngine;
using System.Collections;

public class EarthManager {
	private static EarthManager _instance;
	public static EarthManager Instance
	{
		get
		{
			if(_instance == null)
			{
				_instance = new EarthManager();
			}
			return _instance;
		}
	}
	public int startGuide;

}
