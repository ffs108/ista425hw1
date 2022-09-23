using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///
/// SPACE STATION DEFENSE! (Starter Code)
/// School of Information, University of Arizona
/// A simple 2D game demonstration by Leonard D. Brown
///
/// This code may modified freely for ISTA 425 and 
/// INFO 525 (Algorithms for Games) students in their
/// assignments. Other uses covered by the terms of 
/// the GNU Lesser General Public License (LGPL).
/// 
/// Class Engines provides simple logic for moving and 
/// rotating a GameObject in world space based on its 
/// current position. Provides a listener that responds 
/// to user's invocation of arrow keys (up, down, left, 
/// and right).
/// 
/// </summary>

public class Engines : MonoBehaviour
{
    [Tooltip("Frame-rate independent movement rate in screen units")]
    public float MovementRate = 2.0f;

    [Tooltip("Frame-rate independent rotation rate in degrees")]
    public float RotationRate = 60.0f;

    [Tooltip("Time to fade engine sound after shutoff, in seconds")]
    public float fadeTime = 0.5f;

    public float VelocityDamper = 0.25f;
    public float RotationDamper = 1.0f;

    private float currentMovement;
    private float currentRotation;

    private bool isRotating = false;
    private bool isMoving = false;

    float startVolume;

    // Start is called before the first frame update
    void Start()
    {
        // the engine's user-specified sound preset
        startVolume = GetComponents<AudioSource>()[0].volume;
    }

    float determineMovement(float i, float movement, bool forRotation)
    {
        bool enginesOn = isMoving;
        if (forRotation)
        {
            enginesOn = isRotating;
        }
        if (!enginesOn)
        {
            return 0f;
        }
        else if (i == 1.0f)
        {
            return movement;
        }
        else
        {
            return -movement;
        }
    }

    float modifyMovement(float curMove, float damper, bool isRotating)
    {
        bool isNeg = false;
        if (curMove < 0)
        {
            isNeg = true;
            curMove = Mathf.Abs(curMove);
        }
        if (isRotating)
        {
            damper = damper * 5;
        }
        curMove -= damper / 10;
        if (curMove < 0)
        {
            if (isRotating)
            {
                isRotating = false;
            }
            else
            {
                isMoving = false;
            }
            return 0;
        }
        if (isNeg)
        {
            curMove = -curMove;
        }
        return curMove;
    }

    // Update is called once per frame
    void Update()
    {
        // input from up/down, left/right keys
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        AudioSource impulse = GetComponents<AudioSource>()[0];
        if ((Mathf.Abs(x) > 0.0f || Mathf.Abs(y) > 0.0f))
        {
            // ship is moving; engage the engine (sounds)
            impulse.volume = startVolume;
            if (!impulse.isPlaying)
            {
                impulse.Play();
            }

            if (y != 0.0f)
            {
                isMoving = true;
                currentMovement = determineMovement(y, MovementRate, false);
            }
            if (x != 0.0f)
            {
                isRotating = true;
                currentRotation = determineMovement(-x, RotationRate, true);
            }
        }
        else
        {
            // ship has come to a stop, cease engine (sounds)
            if (impulse.isPlaying)
            {
                if (fadeTime > 0.01)
                    // a linear fade on the sound itself, simple but not very elegant
                    impulse.volume -= (Time.deltaTime / fadeTime);
                else
                    impulse.Stop();
            }

        }
        if (y == 0.0f && isMoving)
        {
            currentMovement = modifyMovement(currentMovement, VelocityDamper, false);
        }
        else if (x == 0.0f && isRotating)
        {
            currentRotation = modifyMovement(currentRotation, RotationDamper, true);
        }

        transform.Rotate(0.0f, 0.0f, 1 * currentRotation * Time.deltaTime);


        // a framerate independent translation along the vector defined by user directional inputs
        transform.Translate((new Vector3(0, 1, 0)).normalized * currentMovement * Time.deltaTime);
    }
}
