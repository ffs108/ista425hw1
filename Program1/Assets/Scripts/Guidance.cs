using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Security.Cryptography;
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
/// Class Guidance provides game logic for moving torpedoes 
/// (projectiles) in a 2D game world. The guidance system
/// selectively acquires targets and initiates an explosion
/// upon impact. Torpedoes have a designer-specified time
/// to live (TTL), after which they will cease to exist.
///
/// Hint: You will need to upgrade this system.
/// 
/// </summary>

public class Guidance : MonoBehaviour
{
    [Tooltip("Speed of the torpedo")]
    public float velocity = 10.0f;

    [Tooltip("Direction of the torpedo")]
    public Vector3 direction = Vector3.zero;

    [Tooltip("Maximum torpedo life in seconds")]
    public float timeToLive = 6.0f;

    [Tooltip("Time for torpedo to fade out in seconds")]
    public float fadeTime = 0.5f;

    GameObject[] potentialTargets = null;
    GameObject targetObject = null;
    Vector3 targetPoint = Vector3.zero;

    bool targetImpacted = false;
    bool targetAcquired = false;

    Vector3 startPos;
    float t = 0.0f;

    float tMax = 0.0f;

    //reflection var
    public float NumberReflections = 0.0f;

    // Acquire a new target point
    bool Acquire(out Vector3 target, ref GameObject targObj)
    {
        bool found = false;
        target = Vector3.zero;

        bool candFound = false;
        Vector3 candTarget = Vector3.zero;

        // finds the first applicable target in the list.
        for (int i = 0; i < potentialTargets.Length && !found; i++)
        {
            // acquire correct target
            //print("Checking target " + potentialTargets[i].name);
            if (potentialTargets[i] != null)
            {
                candFound = potentialTargets[i].GetComponent<Targetable>().Intersect(out candTarget, transform.position, direction);
            }

            if (candFound)
            {
                found = candFound;
                target = candTarget;
                targObj = potentialTargets[i];
            }
        }
        return found;
    }

    // Start is called before the first frame update
    void Start()
    {
        // assemble a list of all targets this torpedo may acquire
        potentialTargets = GameObject.FindGameObjectsWithTag("Targetable");
        // place station object in the first index
        int stationIndex = -1;
        for (int i = 0; i < potentialTargets.Length; i++)
        {
            if (potentialTargets[i].name.Equals("Station Object"))
            {
                stationIndex = i;
            }
        }
        if (stationIndex != -1)
        {
            //print("place station object in front");
            GameObject temp = potentialTargets[0];
            potentialTargets[0] = potentialTargets[stationIndex];
            potentialTargets[stationIndex] = temp;
        }

        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // decrement the lifetime counter for this tick
        timeToLive -= Time.deltaTime;
        //Debug.Log("Time to live: " + timeToLive);

        // move until contact with target or out of gas
        if (!targetImpacted && timeToLive > 0.001f)
        {
            if (!targetAcquired)
            {
                targetAcquired = Acquire(out targetPoint, ref targetObject);
                //print(targetObject.name + " at postion " + targetPoint);
                //startPos = transform.position;
                t = 0.0f;//reset
                //Debug.Log("Target acquired: " + targetAcquired + " at position " + targetPoint);
            }


            // Note: The targeting logic below is imperfect and can cause the
            // torpodo to "miss" its target under certain circumstances. Why?
            //
            // Upgrade this code by using LERP to move to torpedo to its target
            // and detect collisions.

            float currentDist = Mathf.Abs((transform.position - targetPoint).sqrMagnitude); //OLD
            //float currentDist = (myLerp(startPos, targetPoint, (t+Time.deltaTime)/timeToLive)).sqrMagnitude;
            //transform.position = myLerp(startPos, targetPoint, velocity*Time.deltaTime);


            /* LERPing done when target acq'd */
            Vector3 distVec = targetPoint - startPos;
            float length = Mathf.Sqrt(Mathf.Pow(distVec.x, 2) + Mathf.Pow(distVec.y, 2));
            t += (velocity * Time.deltaTime) * 1f / length; //not timeToLive??
                                                            // (1 - t) * v0 + t * v1
            Vector3 lerped = (1 - t) * startPos + t * distVec;

            // Either there's no target, or we haven't yet impacted target.
            if (!targetAcquired || t <= 1f || targetImpacted == true)
            {
                // just move along direction of travel indefinitely
                transform.Translate(direction * velocity * Time.deltaTime);
                //transform.position = (direction * velocity * Time.deltaTime);
            }
            else  // we've hit an acquired target: reflect
            {
                /* Reflection when torpedo hits wall */
                targetImpacted = true;
                // check if bug is hit
                if (targetObject.name.Equals("Space Bug Object"))
                {
                    targetObject.GetComponent<Observer>().SetIsHit(true);
                    Destroy(gameObject);
                }
                if (NumberReflections != -1)
                {
                    Vector3 norm = targetObject.GetComponent<Targetable>().getNormal(); //station wall norm
                    direction = reflection(direction, norm);
                    targetAcquired = false;
                    startPos = transform.position;
                    //this ^ is v' 
                    //transform.position = direction * velocity * Time.deltaTime; //not t.pos ig
                    transform.Translate(direction * velocity * Time.deltaTime);
                    NumberReflections -= 1;
                    // have torpedo die after x hits
                    if (NumberReflections == -1)
                    {
                        timeToLive = fadeTime;
                    }
                    // contacted with targetable object; play ricochset sound
                    AudioSource ricochet = GetComponents<AudioSource>()[1];
                    ricochet.Play();
                }

                // reset t
                t = 0.0f;
            }
            targetImpacted = false;

        }

        if (timeToLive <= fadeTime)
        {
            SpriteRenderer renderer = GetComponent<SpriteRenderer>();

            // fade the torpedo out of existence
            renderer.color = new Color(1.0f, 1.0f, 1.0f, (renderer.color.a - (Time.deltaTime / fadeTime)));

            if (timeToLive <= 0.001f)
                // remove the object from the game
                GameObject.Destroy(gameObject);
        }
    }

    /*
	 *  
	 * Added LERP functions here -ff
	 * 
	 */

    //helper function --unneccesary
    static float lowerOrderLerp(float firstVal, float secondVal, float t)
    {
        return (1 - t) * firstVal + t * secondVal; // (1 - t) * v0 + t * v1
    }

    //hates my func but is ok when in the if statement?????
    static Vector3 myLerp(Vector3 start, Vector3 end, float t)
    {
        Vector3 retval = end - start;
        float length = Mathf.Sqrt(Mathf.Pow(retval.x, 2) + Mathf.Pow(retval.y, 2));
        //t += (Guidance.velocity * Time.deltaTime) * 1f / length; -- all of a sudden velocity could not be called on?
        //retVal =* t + start;
        return retval;
    }

    /*
     * 
     * Added reflection function here -- ff
     * 
     */
    Vector3 reflection(Vector3 start, Vector3 norm)
    {
        float dot = (start.x * norm.x) + (start.y * norm.y) + (start.z * norm.z);
        // v'= v - 2n(v*n)
        Vector3 vecPrime = start - (2.0f * dot * norm);
        return vecPrime;
    }
}
