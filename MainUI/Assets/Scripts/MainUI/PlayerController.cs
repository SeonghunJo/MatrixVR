#if UNITY_EDITOR

using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{

    /// <summary>
    /// The rate of rotation when using the keyboard.
    /// </summary>
    public float RotationRatchet = 45.0f;

    public float RotationAmount = 1.5f;
    /// <summary>
    /// The player's current rotation about the Y axis.
    /// </summary>
    private float YRotation = 0.0f;

    /// <summary>
    /// If true, tracking data from a child OVRCameraRig will update the direction of movement.
    /// </summary>
    public bool HmdRotatesY = true;

    private float MoveScale = 1.0f;
    private Vector3 MoveThrottle = Vector3.zero;
    private OVRPose? InitialPose;

    protected OVRCameraRig CameraController = null;

    private float RotationScaleMultiplier = 1.0f;
    private bool SkipMouseRotation = false;
    private float SimulationRate = 60f;

    void Awake()
    {
        // We use OVRCameraRig to set rotations to cameras,
        // and to be influenced by rotation
        OVRCameraRig[] CameraControllers;
        CameraControllers = gameObject.GetComponents<OVRCameraRig>();

        if (CameraControllers.Length == 0)
            Debug.LogWarning("PlayerController : No OVRCameraRig attached.");
        else if (CameraControllers.Length > 1)
            Debug.LogWarning("PlayerController : More then 1 OVRCameraRig attached.");
        else
            CameraController = CameraControllers[0];

        YRotation = transform.rotation.eulerAngles.y;

    }

    protected virtual void Update()
    {
        UpdateMovement();
    }

    public virtual void UpdateMovement()
    {
        Quaternion ort = (HmdRotatesY) ? CameraController.centerEyeAnchor.rotation : transform.rotation;
        Vector3 ortEuler = ort.eulerAngles;
        ortEuler.z = ortEuler.x = 0f;
        ort = Quaternion.Euler(ortEuler);

        Vector3 euler = transform.rotation.eulerAngles;

        float rotateInfluence = SimulationRate * Time.deltaTime * RotationAmount * RotationScaleMultiplier;

        if (!SkipMouseRotation)
        { 
            euler.y += Input.GetAxis("Mouse X") * rotateInfluence * 3.25f;
            euler.x += Input.GetAxis("Mouse Y") * rotateInfluence * 3.25f;
        }
          
        transform.rotation = Quaternion.Euler(euler);
    }
    /// <summary>
    /// Gets the rotation scale multiplier.
    /// </summary>
    /// <param name="rotationScaleMultiplier">Rotation scale multiplier.</param>
    public void GetRotationScaleMultiplier(ref float rotationScaleMultiplier)
    {
        rotationScaleMultiplier = RotationScaleMultiplier;
    }

    /// <summary>
    /// Sets the rotation scale multiplier.
    /// </summary>
    /// <param name="rotationScaleMultiplier">Rotation scale multiplier.</param>
    public void SetRotationScaleMultiplier(float rotationScaleMultiplier)
    {
        RotationScaleMultiplier = rotationScaleMultiplier;
    }

    /// <summary>
    /// Gets the allow mouse rotation.
    /// </summary>
    /// <param name="skipMouseRotation">Allow mouse rotation.</param>
    public void GetSkipMouseRotation(ref bool skipMouseRotation)
    {
        skipMouseRotation = SkipMouseRotation;
    }

    /// <summary>
    /// Sets the allow mouse rotation.
    /// </summary>
    /// <param name="skipMouseRotation">If set to <c>true</c> allow mouse rotation.</param>
    public void SetSkipMouseRotation(bool skipMouseRotation)
    {
        SkipMouseRotation = skipMouseRotation;
    }

    /// <summary>
    /// Resets the player look rotation when the device orientation is reset.
    /// </summary>
    public void ResetOrientation()
    {
        Vector3 euler = transform.rotation.eulerAngles;
        euler.y = YRotation;
        transform.rotation = Quaternion.Euler(euler);
    }
}


#endif