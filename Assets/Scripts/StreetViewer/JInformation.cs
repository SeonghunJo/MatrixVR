// Use the Unity new GUI with Unity 4.6 or above.
#if UNITY_4_6 || UNITY_5_0
#define USE_NEW_GUI
#endif

using System;
using System.Collections;
using UnityEngine;
#if USE_NEW_GUI
using UnityEngine.UI;
# endif

//OVRMainMenu 형태로 Information을 띄워주는 화면
public class JInformation : MonoBehaviour
{

	public float 	FadeInTime    	= 2.0f;
	public UnityEngine.Texture 	FadeInTexture 	= null;
	public Font 	FontReplace		= null;
    public Font     FontTitle       = null;
	public KeyCode ToggleKey		= KeyCode.I;
	public KeyCode	QuitKey			= KeyCode.Escape;

	public bool ScenesVisible   	= false;
    public bool ScenesVisibleInf    = false;
	
	// Spacing for scenes menu // 1280.0f, 800.0f
	private int resolutionX = 1280;
	private int resolutionY = 800;

	public int height = 800;

	private int screenCenterX = 640;
	private int screenCenterY = 400;

    private int processBarLocation = 0;
    
    public Texture flagImg;

    public int loadingset = 0;

	public InformationList infoList;
	// Handle to OVRCameraRig
	private OVRCameraRig CameraController = null;
	// Handle to OVRPlayerController
	private OVRPlayerController PlayerController = null;
	
	// Replace the GUI with our own texture and 3D plane that
	// is attached to the rendder camera for true 3D placement
	private OVRGUI  		GuiHelper 		 = new OVRGUI();
	private GameObject      GUIRenderObject  = null;
	public RenderTexture	GUIRenderTexture = null;

	private GameObject NewGUIObject                 = null;
	private GameObject RiftPresentGUIObject         = null;

	public string 			LayerName 		 = "Default"; // We can set the layer to be anything we want to, this allows specific camera to render it.

	public UnityEngine.Texture  CrosshairImage 			= null;
	private OVRCrosshair Crosshair        	= new OVRCrosshair();
	// We want to hold onto GridCube, for potential sharing
	// of the menu RenderTarget
	OVRGridCube GridCube = null;
	
	#region MonoBehaviour Message Handlers
	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake()
	{    
		// Find camera controller
		OVRCameraRig[] CameraControllers;
		CameraControllers = gameObject.GetComponentsInChildren<OVRCameraRig>();
		
		if(CameraControllers.Length == 0)
			Debug.LogWarning("OVRMainMenu: No OVRCameraRig attached.");
		else if (CameraControllers.Length > 1)
			Debug.LogWarning("OVRMainMenu: More then 1 OVRCameraRig attached.");
		else{
			CameraController = CameraControllers[0];

			OVRUGUI.CameraController = CameraController;

		}
		
		// Find player controller
		OVRPlayerController[] PlayerControllers;
		PlayerControllers = gameObject.GetComponentsInChildren<OVRPlayerController>();
		
		if(PlayerControllers.Length == 0)
			Debug.LogWarning("OVRMainMenu: No OVRPlayerController attached.");
		else if (PlayerControllers.Length > 1)
			Debug.LogWarning("OVRMainMenu: More then 1 OVRPlayerController attached.");
		else{
			PlayerController = PlayerControllers[0];
			OVRUGUI.PlayerController = PlayerController;

		}

		NewGUIObject = new GameObject();
		NewGUIObject.name = "OVRGUIMain";
		NewGUIObject.transform.parent = GameObject.Find("LeftEyeAnchor").transform;
		RectTransform r = NewGUIObject.AddComponent<RectTransform>();
		r.sizeDelta = new Vector2(100f, 100f);
		r.localScale = new Vector3(0.001f, 0.001f, 0.001f);
		r.localPosition = new Vector3(0.01f, 0.17f, 0.53f);
		r.localEulerAngles = Vector3.zero;
		
		Canvas c = NewGUIObject.AddComponent<Canvas>();
		c.renderMode = RenderMode.WorldSpace;
		c.pixelPerfect = false;
	}
	
	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start()
	{		
		ScenesVisible  = false;

		infoList = new InformationList ();

		// Set the GUI target
		GUIRenderObject = GameObject.Instantiate(Resources.Load("OVRGUIObjectMain")) as GameObject;
		
		if(GUIRenderObject != null)
		{
			// Chnge the layer
			GUIRenderObject.layer = LayerMask.NameToLayer(LayerName);
			
			if(GUIRenderTexture == null)
			{
				int w = Screen.width;
				int h = Screen.height;
				
				// We don't need a depth buffer on this texture
				GUIRenderTexture = new RenderTexture(w, h, 0);	
				GuiHelper.SetPixelResolution(w, h);
				// NOTE: All GUI elements are being written with pixel values based
				// from DK1 (1280x800). These should change to normalized locations so 
				// that we can scale more cleanly with varying resolutions
				GuiHelper.SetDisplayResolution(1280.0f, 800.0f);
			}
		}
		
		// Attach GUI texture to GUI object and GUI object to Camera
		if(GUIRenderTexture != null && GUIRenderObject != null)
		{
			GUIRenderObject.GetComponent<Renderer>().material.mainTexture = GUIRenderTexture;
			
			if(CameraController != null)
			{
				// Grab transform of GUI object
				Vector3 ls = GUIRenderObject.transform.localScale;
				Vector3 lp = GUIRenderObject.transform.localPosition;
				Quaternion lr = GUIRenderObject.transform.localRotation;
				
				// Attach the GUI object to the camera
				GUIRenderObject.transform.parent = CameraController.centerEyeAnchor;
				// Reset the transform values (we will be maintaining state of the GUI object
				// in local state)
				
				GUIRenderObject.transform.localScale = ls;
				GUIRenderObject.transform.localPosition = lp;
				GUIRenderObject.transform.localRotation = lr;
				
				// Deactivate object until we have completed the fade-in
				// Also, we may want to deactive the render object if there is nothing being rendered
				// into the UI
				GUIRenderObject.SetActive(false);
			}
		}
		
		// Make sure to hide cursor 
		if(Application.isEditor == false)
		{
			#if UNITY_5_0
			Cursor.visible = false; 
			Cursor.lockState = CursorLockMode.Locked;
			#else
			Screen.showCursor = false; 
			Screen.lockCursor = true;
			#endif
		}
		
		// CameraController updates
		if(CameraController != null)
		{
			// Add a GridCube component to this object
			GridCube = gameObject.AddComponent<OVRGridCube>();
			GridCube.SetOVRCameraController(ref CameraController);

		}
		
		// Crosshair functionality
		Crosshair.Init();
		Crosshair.SetCrosshairTexture(ref CrosshairImage);
		Crosshair.SetOVRCameraController (ref CameraController);
		Crosshair.SetOVRPlayerController(ref PlayerController);

		print("start function : " + ScenesVisible);
		StartCoroutine(ShowInformation(true));
	} 
	
	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update()
	{		
		// CameraController updates
		if(CameraController != null)
		{
			UpdateRecenterPose();

		}

		// Crosshair functionality
		Crosshair.UpdateCrosshair();
		
		if (ScenesVisible)
		{
			NewGUIObject.SetActive(true);

		}
		else
		{
			NewGUIObject.SetActive(false);
		}
		
		// Toggle Fullscreen
		if(Input.GetKeyDown(KeyCode.F11))
			Screen.fullScreen = !Screen.fullScreen;
		
		if (Input.GetKeyDown(KeyCode.M))
			OVRManager.display.mirrorMode = !OVRManager.display.mirrorMode;
		
		ToggleScreenByKey(ToggleKey);

		#if !UNITY_ANDROID || UNITY_EDITOR
		// Escape Application
		if (Input.GetKeyDown(QuitKey))
			Application.Quit();
		#endif

	}
	
	
	void OnGUI()
	{	
		// Important to keep from skipping render events
		if (Event.current.type != EventType.Repaint)
			return;
		
		// We can turn on the render object so we can render the on-screen menu
		if(GUIRenderObject != null)
		{
			if (ScenesVisible || 
			    Crosshair.IsCrosshairVisible() )
			{
				GUIRenderObject.SetActive(true);
			}
			else
			{
				GUIRenderObject.SetActive(false);
			}
		}
		
		//***
		// Set the GUI matrix to deal with portrait mode
		Vector3 scale = Vector3.one;
		Matrix4x4 svMat = GUI.matrix; // save current matrix
		// substitute matrix - only scale is altered from standard
		GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, scale);
		
		// Cache current active render texture
		RenderTexture previousActive = RenderTexture.active;
		
		// if set, we will render to this texture
		if(GUIRenderTexture != null && GUIRenderObject.activeSelf)
		{
			RenderTexture.active = GUIRenderTexture;
			GL.Clear (false, true, new Color (0.0f, 0.0f, 0.0f, 0.0f));
		}
		
		// Update OVRGUI functions (will be deprecated eventually when 2D renderingc
		// is removed from GUI)
		GuiHelper.SetFontReplace(FontReplace);
		
		DrawScreen();

		// The cross-hair may need to go away at some point, unless someone finds it 
		// useful
		Crosshair.OnGUICrosshair();
		
		// Since we want to draw into the main GUI that is shared within the MainMenu,
		// we call the OVRVisionGuide GUI function here
		//VisionGuide.OnGUIVisionGuide();
		
		// Restore active render texture
		if (GUIRenderObject.activeSelf)
		{
			RenderTexture.active = previousActive;
		}
		
		// ***
		// Restore previous GUI matrix
		GUI.matrix = svMat;
	}
	#endregion
	
	
	void UpdateRecenterPose()
	{
		if(Input.GetKeyDown(KeyCode.R))
		{
			OVRManager.display.RecenterPose();
		}
	}

	#region Information output
	public void ShowScreen()
	{
		ScenesVisible = true;

	}
	
	public void HideScreen()
	{
		ScenesVisible = false;

	}
	
	public bool ToggleScreenByKey(KeyCode keyCode)
	{
		bool startPressed = Input.GetKeyDown(keyCode);
		
		if (startPressed)
		{
			print("Toggle");
			ScenesVisible = !ScenesVisible;
			if(!ScenesVisible)
			{	
				loadingset = 0;
				height = 800;
			}
		}
		
		return ScenesVisible;
	}
	
	
	// 모든 UI Draw 작업은 이 함수에 추가
	void DrawScreen()
	{
		if(ScenesVisible) // 화면이 보이면
		{
			GUIDrawLoadingScreen();
		}
		else
		{
			
		}
	}
	IEnumerator ShowInformation(bool forceStart)
	{
		while(forceStart)
		{
			if(ScenesVisible) {
				yield return new WaitForSeconds(0.5f);
				loadingset++;
			}
			else
				yield return new WaitForSeconds(0.5f);
		}        
	}

	//텍스트
	void GUIDrawLoadingScreen()
	{   
		int index = 0;// = EarthManager.Instance.viewIndex;
		
		//배경 
		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), FadeInTexture);
		
		//국기 이미지 (좌측상단)
		GuiHelper.StereoDrawTexture(screenCenterX - 180, 150, 120, 80, ref flagImg, new Color(0.5f, 0.5f, 0.5f, 1f));  
		
		//국가명, 지역명, 설명 불러옴.
		string country = infoList.placelist [index].country;
		string area = infoList.placelist [index].area;
		string feature = infoList.placelist [index].feature;

		//국가명 출력
		GuiHelper.StereoBox(screenCenterX, 150, 200 , 30, ref country, Color.black);

		//지역명 출력
		GuiHelper.StereoBox(screenCenterX, 200, 200 , 30, ref area, Color.black);

		int start = 0;
		int length = 15;
		int totalLength = feature.Length;
		bool loop = true;


		//설명 출력
		while (start+length < totalLength) {
			string sub = feature.Substring (start, length);

			GuiHelper.StereoBox (screenCenterX - 200, 250, 400, 200, ref sub, Color.black);

			start += length;
		}
		while (start+length > totalLength) {
			length--;
		}
		GuiHelper.StereoBox (screenCenterX - 200, 250, 400, 200, ref feature.Substring(start,length), Color.black);

		//GuiHelper.StereoBox(screenCenterX-200, 250, 400 , 200, ref feature, Color.black);
		//GuiHelper.StereoBox(screenCenterX-200, 250, 400 , 200, ref feature, Color.black);


		//int totalLine = line * 30;

		//if (height > 300 - (totalLine / 2))
		//				height -= loadingset * 4;

		//GuiHelper.StereoBox(screenCenterX-200, 300-(totalLine/2), 400 , totalLine, ref text, Color.white);
		//GuiHelper.StereoBox(screenCenterX-200, height, 400 , totalLine, ref text, Color.black);


	}
	#endregion Information output

	void OnDestroy() // Initialize OVRUGUI on OnDestroy
	{
		OVRUGUI.InitUIComponent = false;
	}
}
