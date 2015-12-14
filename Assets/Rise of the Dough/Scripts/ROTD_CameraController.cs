using UnityEngine;
using System.Collections;

/// <summary>
/// This class controls the camera's movement, making it slowly pan to follow the chef. The bounds keep the 
/// camera from straying offscreen.
/// </summary>
public class ROTD_CameraController : MonoBehaviour 
{
	/// <summary>
	/// The cached transform of the camera
	/// </summary>
	private Transform _transform;

    /// <summary>
    /// The position of the camera
    /// </summary>
	private Vector3 _position;
	
    /// <summary>
    /// A reference to the main camera in the scene
    /// </summary>
	public Camera mainCamera;

    /// <summary>
    /// A reference to the transform to follow (offset from the camera)
    /// </summary>
	public Transform cameraFollow;
	
    /// <summary>
    /// The speed with which to follow the camera
    /// </summary>
	public float cameraFollowSpeed;

    /// <summary>
    /// The screen bounds that contain the camera
    /// </summary>
	public Rect cameraBounds;
	
    /// <summary>
    /// Called once at the start of the scene
    /// </summary>
	void Start () 
	{
        // cache the transform of the camera for quicker lookup
		_transform = mainCamera.transform;
	}
	
    /// <summary>
    /// Called each frame after all other updates
    /// </summary>
	void LateUpdate () 
	{
        // get the current position
		_position = _transform.position;
		
		// make the camera follow smoothly with a slight delay in the player's movement
		_position = Vector3.Lerp(_position, cameraFollow.position, cameraFollowSpeed);
		
		// make sure the camera doesn't stray too far left or right
		_position.x = Mathf.Max (_position.x, cameraBounds.xMin);
		_position.x = Mathf.Min (_position.x, cameraBounds.xMax);
		_position.y = Mathf.Max (_position.y, cameraBounds.yMin);
		_position.y = Mathf.Min (_position.y, cameraBounds.yMax);
		
        // update the current position
		_transform.position = _position;
	}
}
