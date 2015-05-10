using UnityEngine;
using System.Collections;

public class LoadingScreen : MonoBehaviour {

	public string levelToLoad;

	public GameObject background;
	public GameObject text;
	public GameObject progressBar;

	private int loadProgress = 0;

	// Use this for initialization
	void Start () {
		background.SetActive(false);
		text.SetActive(false);
		progressBar.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown("space")) {
			StartCoroutine(DisplayLoadingScreen(levelToLoad));
		}
	}

	IEnumerator DisplayLoadingScreen(string level) {
		background.SetActive(true);
		text.SetActive(true);
		progressBar.SetActive(true);

		progressBar.transform.localScale = new Vector3(loadProgress, progressBar.transform.localScale.y, progressBar.transform.localScale.z);

		text.guiText.text = "Loading Progress " + loadProgress + "%";

		AsyncOperation async = Application.LoadLevelAsync(levelToLoad);

		while (!async.isDone) {
			loadProgress = (int)(async.progress * 100);
			text.guiText.text = "Loading Progress " + loadProgress + "%";
			progressBar.transform.localScale = new Vector3(loadProgress, progressBar.transform.localScale.y, progressBar.transform.localScale.z);
			
			yield return null;
		}
	}
}
/*
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
*/
