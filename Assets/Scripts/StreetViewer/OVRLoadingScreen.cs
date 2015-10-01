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

public class OVRLoadingScreen : MonoBehaviour
{
	public float 	FadeInTime    	= 2.0f;
	public UnityEngine.Texture 	FadeInTexture 	= null;
	public Font 	FontReplace		= null;
    public Font     FontTitle       = null;
	public KeyCode ToggleKey		= KeyCode.Space;
	public KeyCode	QuitKey			= KeyCode.Escape;

	public bool ScenesVisible   	= false;
    public bool ScenesVisibleInf    = false;
	
	// Spacing for scenes menu // 1280.0f, 800.0f
	private int resolutionX = 1280;
	private int resolutionY = 800;

	private int screenCenterX = 640;
	private int screenCenterY = 400;

    private int processBarLocation = 0;
    
    public Texture loadingImg1;
    public Texture loadingImg2;
    public Texture bar;
    public Texture foot1;
    public Texture foot2;

    public int loadingset = 0;

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

		// Create canvas for using new GUI
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
				//GuiHelper.SetDisplayResolution(1920.0f, 1080.0f);
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
        StartCoroutine(DrawWalker(true));
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
			if (ScenesVisible || Crosshair.IsCrosshairVisible())
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

	#region SeonghunJo Added
	public void ShowScreen()
	{
		ScenesVisible = true;
	}

	public void HideScreen()
	{
		ScenesVisible = false;
        loadingset = 0;
	}
	
	public bool ToggleScreenByKey(KeyCode keyCode)
	{
		bool startPressed = Input.GetKeyDown(keyCode);
		
		if (startPressed)
		{
			print("Toggle");
			ScenesVisible = !ScenesVisible;
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

    IEnumerator DrawWalker(bool forceStart)
    {
        while(ScenesVisible || forceStart)
        {
            yield return new WaitForSeconds(0.5f);

            loadingset++;           
        }        
    }

    //애니메이션 효과 추가, process bar,% 추가 
	void GUIDrawLoadingScreen()
	{   
		int boxWidth = 300;
		int boxHeight = 50;
   
		GUI.color = new Color(0, 0, 0);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), FadeInTexture);

        GuiHelper.SetFontReplace(FontTitle);

        //Draw Loading text
		string loading = "L O A D I N G ...";
        GuiHelper.StereoBox(screenCenterX - boxWidth / 2, 340, boxWidth, boxHeight + 5, ref loading, Color.white);

        //Draw People
        if(loadingset % 2 ==0)
            GuiHelper.StereoDrawTexture(screenCenterX - 40, 400, 80, 80, ref loadingImg1, new Color(0.5f, 0.5f, 0.5f, 1f));                                                                                   
        else
            GuiHelper.StereoDrawTexture(screenCenterX - 40, 400, 80, 80, ref loadingImg2, new Color(0.5f, 0.5f, 0.5f, 1f));
        
        //Draw Progress
        int process = Manager.Instance.GetProgress() * 3;
        if (process > 100)
            process = 100;
        string locationText = process.ToString() + " %";
        GuiHelper.StereoBox(screenCenterX - 40, 480, 80, boxHeight, ref locationText, Color.white);

        //Draw Progress bar
        processBarLocation = process * 3;
        GuiHelper.StereoDrawTexture(screenCenterX - 150, 540, processBarLocation, boxHeight, ref bar, new Color(0.5f, 0.5f, 0.5f, 1f));


        string text = EarthManager.Instance.thumbnailText;
        if (text == null)
            text = "M A T R I X";

        //text = "만장굴, South Korea, 제주시, 제주특별자치도";
        GuiHelper.StereoBox(screenCenterX - 200, 300, 400 , 40, ref text, Color.white);
        


	}
	#endregion SeonghunJo Added


	void OnDestroy() // Initialize OVRUGUI on OnDestroy
	{
		OVRUGUI.InitUIComponent = false;
	}
}
