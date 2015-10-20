using UnityEngine;

/// <summary>
/// delegate to be used in other classes when a touch event occurs
/// </summary>
public delegate void TouchPadDelegate(ROTD_TouchPad.TOUCH_EVENT touchEvent, Vector2 screenPosition, Vector3 guiPosition, Vector3 worldPosition);

/// <summary>
/// Captures touch input on the screen and sends the information to a delegate
/// </summary>
public class ROTD_TouchPad : MonoBehaviour
{
	/// <summary>
	/// cached transform for quicker access later
	/// </summary>
	private Transform _thisTransform;
	
	/// <summary>
	/// the delegate to call when a touch event occurs 
	/// </summary>
	private TouchPadDelegate _touchPadDelegate;
	
	/// <summary>
	/// position of the touch 
	/// </summary>
	private Vector3 _guiPosition;
	
	private Vector3 _worldPosition;
	
	/// <summary>
	/// cached value of half the touch pad width 
	/// </summary>
	private float _halfTouchPadWidth;
	
	/// <summary>
	/// cached value of half the touch pad height 
	/// </summary>
	private float _halfTouchPadHeight;
	
	/// <summary>
	/// event that occured 
	/// </summary>
	public enum TOUCH_EVENT
	{
		TouchDown,
		TouchMove,
		TouchUp,
        TouchStationary
	}
	
	/// <summary>
	/// the camera used to translate points
	/// </summary>
	public Camera guiCamera;
	
	public Camera mainCamera;
	
	/// <summary>
	/// width of the touch pad 
	/// </summary>
	public float touchPadWidth;
	
	/// <summary>
	/// height of the touch pad 
	/// </summary>
	public float touchPadHeight;
	
	/// <summary>
	/// Initialize 
	/// </summary>
	void Awake()
	{
		// cache the transform
		_thisTransform = this.transform;
		
		_halfTouchPadWidth = touchPadWidth / 2.0f;
		_halfTouchPadHeight = touchPadHeight / 2.0f;
	}
	
	/// <summary>
	/// Called every frame from the input manager
	/// </summary>
	public void FrameUpdate()
	{
		// Determine whether we are using a mouse or touchpad
		
#if UNITY_IPHONE || UNITY_ANDRIOD
		if (!Application.isEditor)
		{
			CheckTouch();
		}
		else
		{
			CheckMouse();
		}
#else
		
		CheckMouse();		
#endif
	}
	
	public void CheckMouse()
	{
		// if left mouse button is down
		if (Input.GetMouseButtonDown(0))
		{
			// if there is a delegate assigned to handle the input
			if (_touchPadDelegate != null)
			{
				// if position is in bounds
				if (GetTouchPosition(out _guiPosition, out _worldPosition))
				{
					// call the delegate with the touch down event and position
					_touchPadDelegate(TOUCH_EVENT.TouchDown, Input.mousePosition, _guiPosition, _worldPosition);
				}
			}
		}
		else if (Input.GetMouseButtonUp(0))
		{
			// left mouse button released
			
			// if delegate exists, call the touch up event
			if (_touchPadDelegate != null)
				if (GetTouchPosition(out _guiPosition, out _worldPosition))
                {
                    _touchPadDelegate(TOUCH_EVENT.TouchUp, Input.mousePosition, _guiPosition, _worldPosition);
                }
		}
		else if (Input.GetMouseButton(0))
		{
			// left mouse button held down
			
			// if delegate exists and the touch is in bounds, then call the touch move event
			if (_touchPadDelegate != null)
				if (GetTouchPosition(out _guiPosition, out _worldPosition))
				{
					_touchPadDelegate(TOUCH_EVENT.TouchMove, Input.mousePosition, _guiPosition, _worldPosition);
				}
		}		
	}
	
	public void CheckTouch()
	{
		// if a touch began
		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) 
		{
			// if there is a delegate assigned to handle the input
			if (_touchPadDelegate != null)
			{
				// if position is in bounds
				if (GetTouchPosition(out _guiPosition, out _worldPosition))
				{
					// call the delegate with the touch down event and position
					_touchPadDelegate(TOUCH_EVENT.TouchDown, Input.mousePosition, _guiPosition, _worldPosition);
				}
			}				
		}			
		else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) 
		{
			// else if the touch ended

			// if delegate exists, call the touch up event
			if (_touchPadDelegate != null)
				if (GetTouchPosition(out _guiPosition, out _worldPosition))
                {
                    _touchPadDelegate(TOUCH_EVENT.TouchUp, Input.mousePosition, _guiPosition, _worldPosition);
                }
		}
		else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved) 
		{
			// else the touch moved
			
			// if delegate exists and the touch is in bounds, then call the touch move event
			if (_touchPadDelegate != null)
				if (GetTouchPosition(out _guiPosition, out _worldPosition))
				{
					_touchPadDelegate(TOUCH_EVENT.TouchMove, Input.mousePosition, _guiPosition, _worldPosition);
				}
		}
        else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Stationary)
        {
            // else touch is stationary

            // if delegate exists and the touch is in bounds, then call the touch move event
            if (_touchPadDelegate != null)
				if (GetTouchPosition(out _guiPosition, out _worldPosition))
                {
                    _touchPadDelegate(TOUCH_EVENT.TouchStationary, Input.mousePosition, _guiPosition, _worldPosition);
                }
        }
	}
	
	/// <summary>
	/// Determines if a touch is in the rectangle's bounds
	/// </summary>
	/// <param name="position">
	/// Camera relative position
	/// </param>
	/// <returns>
	/// Returns true if in bounds
	/// </returns>	
	private bool GetTouchPosition(out Vector3 guiPosition, out Vector3 mainPosition)
	{
		// translate the position to world space
		guiPosition = guiCamera.ScreenToWorldPoint(Input.mousePosition);
		mainPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
		guiPosition.z = 0;
		
		if (guiPosition.x >= (_thisTransform.position.x - _halfTouchPadWidth) && guiPosition.x <= (_thisTransform.position.x + _halfTouchPadWidth)
		    	&& guiPosition.y >= (_thisTransform.position.y - _halfTouchPadHeight) && guiPosition.y <= (_thisTransform.position.y + _halfTouchPadHeight))
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	
	/// <summary>
	/// Sets the touch delegate from another class 
	/// </summary>
	/// <param name="_touchPadDelegate">
	/// A <see cref="TouchPadDelegate"/>
	/// </param>
	public void SetTouchPadDelegate(TouchPadDelegate touchPadDelegate)
	{
		_touchPadDelegate = touchPadDelegate;
	}
	
	/// <summary>
	/// Draws a wireframe cube of the touchpad in the scene editor 
	/// </summary>
	void OnDrawGizmosSelected() 
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireCube(transform.position, new Vector3(touchPadWidth, touchPadHeight, 0.1f));
	}
}