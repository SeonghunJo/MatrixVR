using UnityEngine;
using System.Collections;

// 모든 객체 및 스크립트에서 공용으로 접근 가능한 싱글턴 패턴 클래스
// SINGLETON MANAGER CLASS FOR ALL SCENES AND SCRIPT 

public class Manager {

	private static Manager _instance;
	public static Manager Instance
	{
		get
		{
			if(_instance == null)
			{
				_instance = new Manager();
			}
			return _instance;
		}
	}
	// FOR STREETVIEWER.SCENE VARIABLES AND FUNCTIONS
	public string panoramaID;
	public string[] nextIDs;
    public string[] nextDegrees;

    public int processCount;

    // FOR MainUI.scene VARIABLES AND FUNCTIONS

    public string thumbnailID;
    public string thumbnailText;
    public Texture2D thumbnailImg;

}