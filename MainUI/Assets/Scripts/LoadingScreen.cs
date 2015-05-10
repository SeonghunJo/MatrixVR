using UnityEngine;
using System.Collections;

public class LoadingScreen : MonoBehaviour {

	public Texture2D texture;
	static LoadingScreen _instance;

	void Awake()
	{
		if (_instance)
		{
			Destroy(gameObject);
			return;
		}
		_instance = this;
		gameObject.AddComponent<GUITexture>().enabled = false;
		guiTexture.texture = texture;
		transform.position = new Vector3(0.5f, 0.5f, 0.0f);
		DontDestroyOnLoad(this);
	} 

	public static void Load(int index)
	{
		if(NoInstance()) return;
		_instance.guiTexture.enabled = true;
		Application.LoadLevel(index);
		_instance.guiTexture.enabled = false;
	}

	public static void Load(string name)
	{
		if(NoInstance()) return;
		_instance.guiTexture.enabled = true;
		Application.LoadLevel(name);
		_instance.guiTexture.enabled = false;
	}

	public static void Show()
	{
		if(NoInstance()) return;
		_instance.guiTexture.enabled = true;
	}

	public static void Hide()
	{
		if(NoInstance()) return;
		_instance.guiTexture.enabled = false;
	}

	static bool NoInstance()
	{
		if(!_instance)
		{
			Debug.LogError("Loading Screen is not existing in scence.");
			return false;
		}
		return true;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
