using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// This class uses a bone animation to set a numeric value. Each place holder (ones, tens, thousands, etc)
/// is represented by a bone and an animation clip. Each bone is the physical location of the digit. Each animation
/// allows us to set the value of the digit. Note that each animation that sets the digit's place is blended
/// so that we can set the digits individually without having to have millions of animations. The animations
/// can be played simultaneously so that each digit is a unique value.
/// 
/// Score is inherited from the FX class, but it can be used as a stand-alone object. It is used as an FX for
/// the individual scores from destroying pasta and in these cases the FX_00 animation is automatically played (moving
/// the score upward). For the GUI's total score, the FX class portions are ignored and the SetScoreAndBounce 
/// function is called instead.
/// 
/// CAUTION: this is just for an example of different ways you can use blending to achieve some interesting effects.
/// This is not optimized since each score will require a separate draw call since each has its own bone animation.
/// You should use a GUI or 2D package for your GUI elements to reduce draw calls.
/// </summary>
public class ROTD_Score : ROTD_FX 
{
    /// <summary>
    /// Internal storage of the score value in string format
    /// </summary>
    private string _score;

    /// <summary>
    /// Whether or not to center the score text on the position
    /// </summary>
    public bool center;

    /// <summary>
    /// The width of one digit (used for centering the score)
    /// </summary>
    public float digitWidth;

    /// <summary>
    /// Number of digits this score can have
    /// </summary>
    public int digitCountLimit;

    /// <summary>
    /// The integer value of the score
    /// </summary>
    public int IntValue
    {
        get
        {
            if (_score == "")
                return 0;
            else
                return Convert.ToInt32(_score);
        }
    }

    /// <summary>
    /// String value of the score
    /// </summary>
    public override string Value
    {
        get
        {
            return _score;
        }
        set
        {
            string maxScore = "";
            for (int d = 1; d <= digitCountLimit; d++)
            {
                maxScore += "9";
            }

            // clamp the score so that it doesn't exceed the total number of digits
            _score = Mathf.Clamp(Convert.ToInt32(value), 0, Convert.ToInt32(maxScore)).ToString();

            // keep track of how many digits are being used
            int activePlaceCount = 0;
            int frame;

            // loop through each place holder of the score (ones, tens, thousands, etc)
            for (int placeIndex = 0; placeIndex < digitCountLimit; placeIndex++)
            {
                // check whether the score has this place holder
                if (_score.Length > placeIndex)
                {
                    // the number is long enough to need this digit, so set the normalized time based on the digit

                    activePlaceCount++;

                    // show the place holder bone and stop the place holder animation
                    boneAnimation.HideBone("Place_" + placeIndex.ToString(), false);
                    boneAnimation.Stop("Place_" + placeIndex.ToString());

                    // play the place holder animation and set the digit by setting the normalized time of the animation
                    boneAnimation.Play("Place_" + placeIndex.ToString());
                    frame = Convert.ToInt16(_score.Substring(_score.Length - (placeIndex + 1), 1));
                    boneAnimation["Place_" + placeIndex.ToString()].normalizedTime = (float)frame / 9.0f;

                    // we have to force the animation event since the fps = 0. Unity will not fire animation events on zero speed animations.
                    boneAnimation.ForceAnimationEvent("Place_" + placeIndex.ToString(), frame);
                }
                else
                {
                    // the number isn't long enough to need this digit, so hide the place bone and stop the place animation

                    boneAnimation.HideBone("Place_" + placeIndex.ToString(), true);
                    boneAnimation.Stop("Place_" + placeIndex.ToString());
                }
            }

            // hide the commas based on the active digits visible
            if (digitCountLimit > 3)
                boneAnimation.HideBone("Comma_1", (activePlaceCount < 4));

            if (digitCountLimit > 6)
                boneAnimation.HideBone("Comma_2", (activePlaceCount < 7));

            // if we are centering this, move the x position right by half the visible width
            if (center && _thisTransform != null)
            {
                float totalWidth = (activePlaceCount * digitWidth);

                Vector3 position = _originalPosition;
                position.x += (totalWidth / 2.0f);
                _thisTransform.position = position;
            }
        }
    }

    /// <summary>
    /// Called once before other scripts
    /// </summary>
    void Awake()
    {
        // cache the transform for quicker lookup
        _thisTransform = boneAnimation.transform;

        // cache the original position in case this is not used as an FX object
        _originalPosition = _thisTransform.position;
    }

    /// <summary>
    /// Sets the score value and bounces the score to show that it has changed.
    /// Note that the Bounce animation is not blended and plays in the background.
    /// All the digit animations play blended on top of the Bounce animation.
    /// </summary>
    /// <param name="score">The score value to set</param>
    public void SetScoreAndBounce(int score)
    {
        boneAnimation.Play("Bounce");
        Value = score.ToString();
    }
}
