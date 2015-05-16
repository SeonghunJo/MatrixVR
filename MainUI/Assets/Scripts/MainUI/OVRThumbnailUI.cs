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

public class OVRThumbnailUI : MonoBehaviour
{
	public float 	FadeInTime    	= 2.0f;
	public UnityEngine.Texture 	FadeInTexture 	= null;
	public Font 	FontReplace		= null;
	public KeyCode ToggleKey		= KeyCode.Space;
	public KeyCode	QuitKey			= KeyCode.Escape;
	
	public bool ScenesVisible   	= false;
	
	// Spacing for scenes menu
	private int    	resolutionX			= 1280;
	private int    	resolutionY			= 800;
	
	// Handle to OVRCameraRig
	private OVRCameraRig CameraController = null;
	
	// Handle to OVRPlayerController
	private OVRPlayerController PlayerController = null;
	
	// Rift detection
	private bool   HMDPresent           = false;
	private string strRiftPresent		= "";
	
	// Replace the GUI with our own texture and 3D plane that
	// is attached to the rendder camera for true 3D placement
	private OVRGUI  		GuiHelper 		 = new OVRGUI();
	private GameObject      GUIRenderObject  = null;
	public RenderTexture	GUIRenderTexture = null;
	
	private GameObject NewGUIObject                 = null;
	private GameObject RiftPresentGUIObject         = null;
	

	/// We can set the layer to be anything we want to, this allows
	/// a specific camera to render it.
	public string 			LayerName 		 = "Default";
	
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
			#if USE_NEW_GUI
			OVRUGUI.CameraController = CameraController;
			#endif
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
			#if USE_NEW_GUI
			OVRUGUI.PlayerController = PlayerController;
			#endif
		}
		
		#if USE_NEW_GUI
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
		#endif
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
		
		#if USE_NEW_GUI
		if (ScenesVisible)
		{
			NewGUIObject.SetActive(true);
		}
		else
		{
			NewGUIObject.SetActive(false);
		}
		#endif
		
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
	
	#region Jin Added
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
		}
		
		return ScenesVisible;
	}
	
	
	// 모든 UI Draw 작업은 이 함수에 추가
	void DrawScreen()
	{

		if(ScenesVisible) // 화면이 보이면
		{
			DrawThumbnailImg();
		}
		else
		{
			
		}
	}
	void DrawThumbnailImg()
	{
		int width = 160;
		int height = 80;
		Texture image = Manager.Instance.thumbnailImg;
		GuiHelper.StereoDrawTexture(resolutionX/2 - width/2, resolutionY-500, width, height, ref image, new Color(0.5f, 0.5f, 0.5f, 1f));

		int textWidth = 400;
		int textHeight = 30;
		string locationName = Manager.Instance.thumbnailText;

		GuiHelper.StereoBox (resolutionX/2 - textWidth/2, resolutionY-400, textWidth, textHeight, ref locationName, Color.white);
	}
	#endregion Jin Added
	
	/// <summary>
	/// Initialize OVRUGUI on OnDestroy
	/// </summary>
	void OnDestroy()
	{
		#if USE_NEW_GUI
		OVRUGUI.InitUIComponent = false;
		#endif
	}
}
