using UnityEngine;
using System.Collections;
using SmoothMoves;

/// <summary>
/// This class uses an animation to simulate a GUI progress bar. This shows how you can use
/// the normalized time to set the progress value of an animation.
/// 
/// CAUTION: this is not the most efficient way to handle a progress bar. Use a GUI or 2D
/// package for this to reduce your draw calls.
/// </summary>
public class ROTD_ProgressBar : MonoBehaviour 
{
    /// <summary>
    /// Reference to the animation
    /// </summary>
    public BoneAnimation progressBarAnimation;

    /// <summary>
    /// Sets the progress value
    /// </summary>
    public float Progress
    {
        set
        {
            // clamp the progress between zero and one
            value = Mathf.Clamp01(value);

            if (value == 0)
            {
                // the value is zero, so we play the empty animation

                progressBarAnimation.Play("Empty");
            }
            else if (value == 1.0f)
            {
                // the value is one, so we play the full animation

                progressBarAnimation.Play("Full");
            }
            else
            {
                // the value is between zero and one, so we play the progress animation
                // and set the normalized time to the value

                progressBarAnimation.Play("Progress");
                progressBarAnimation["Progress"].normalizedTime = value;
            }
        }
    }

    /// <summary>
    /// Called at the start of the scene
    /// </summary>
    void Start()
    {
        // set the progress to full
        Progress = 1.0f;

        ResetToStart();
    }

    /// <summary>
    /// Resets the progress bar to the start state
    /// </summary>
    public void ResetToStart()
    {
        progressBarAnimation.Stop();
        Progress = 1.0f;
    }

}
