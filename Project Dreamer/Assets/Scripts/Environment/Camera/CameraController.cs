using System;
using Unity.Cinemachine;
using Unity.Mathematics;    
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Splines;

public class CameraController : MonoBehaviour
{
    CinemachineSplineDolly spline;
    CinemachineCamera cam;
    GameObject player;
    public bool playerColliding = false;
    //public List<GameObject> deadZone;
    //CinemachineTargetGroup group;
    //Transform currentLookAt;

    public bool transitioning = false;
    public int goal = 0;
    public float startPos;
    /// <summary>
    /// A value between 0-1 that determines how far along in a transition the camera is
    /// </summary>
    float time = 0;
    float defaultTime = 4f;
    float timeToMove;
    public float maxDistance = 30;
    public float rotationSpeed = 10;
    public List<Knot> knots = new List<Knot>();


    void Start()
    {
        spline = this.GetComponent<CinemachineSplineDolly>();
        cam = this.GetComponent<CinemachineCamera>();
        player = GameObject.Find("Player");
        timeToMove = defaultTime;
        foreach (UnityEngine.Splines.DataPoint<CinemachineSplineDollyLookAtTargets.Item> i in cam.GetComponent<CinemachineSplineDollyLookAtTargets>().Targets)
        {
            knots.Add(i.Value.LookAt.gameObject.GetComponentInParent<Knot>());
        }
        
        /*for (int i = 0; i < knots.Length; i++)
        {
            CinemachineSplineDollyLookAtTargets.Item b = new CinemachineSplineDollyLookAtTargets.Item();
            b.LookAt = knots[i].lookAt.transform;
            cam.GetComponent<CinemachineSplineDollyLookAtTargets>().Targets.Add(i, b);
        }*/

        //BoxCollider[] temp = this.GetComponentsInChildren<BoxCollider>();
        //foreach (BoxCollider b in temp)
        //    deadZone.Add(b.gameObject);


        /*group = GetComponentInChildren<CinemachineTargetGroup>();
        lookAt.Weight = 1;
        lookAt.Radius = 0.5f;
        lookAt.Object = knots[0].lookAt;*/
    }

    void FixedUpdate()
    {
        //currentLookAt = knots[(int)spline.CameraPosition].lookAt;
        if (transitioning)
        {
            // ease in ease out
            time += Time.deltaTime * (1/timeToMove);
            float t = time * time / (2.0f * ((time * time) - time) + 1.0f);
            
            spline.CameraPosition = Mathf.Lerp(startPos, goal, t);


            // If camera has reached the knot (finish transition)
            if (Mathf.Max(spline.CameraPosition, goal) - Mathf.Min(spline.CameraPosition, goal) <= 0.005) {
                transitioning = false;
                startPos = -10;
                time = 0;
                spline.CameraPosition = goal;
                timeToMove = defaultTime;
                player.GetComponent<PlayerMovement>().SetToTransitioningSpeed(false);
            }
        }
        /*else if (cam.transform.forward != (currentLookAt.position - cam.transform.position).normalized && !playerColliding) {
            cam.Target.LookAtTarget = currentLookAt; 

            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(currentLookAt.position - cam.transform.position), rotationSpeed * Time.deltaTime);


            Debug.Log(currentLookAt.position - cam.transform.position + " " + cam.transform.forward);
        }
        if (playerColliding)
        {
            cam.Target.LookAtTarget = player.transform;
        }*/
            float distance = (player.transform.position - cam.transform.position).magnitude;
        if (distance > maxDistance) {
            cam.Lens.FieldOfView = 75 - (distance - maxDistance);
        }
    }

    /// <summary>
    /// Method that the triggers call to set the relevant camera variables and start its movement
    /// </summary>
    /// <param name="index">Whether the camera should be moving forwards or backwards (should only ever be -1 or 1)</param>
    /// <param name="t">The time it should take to transition (if less than zero, use default) </param>
    public void MoveTo(int index, float t)
    {
        goal = index;
        Debug.Log(index);
        //group.Targets[0].Object = knots[index].lookAt;

        // If the camera is mid-transition handle time and goal assignment
        if (transitioning)
        {
            startPos = spline.CameraPosition;
            timeToMove = math.abs(goal - startPos) * defaultTime;
            time = 0;
            Debug.Log(startPos + " to " + goal);
        }
        else
        {
            player.GetComponent<PlayerMovement>().SetToTransitioningSpeed(true);
            startPos = spline.CameraPosition;
            if (t >= 0)
                timeToMove = t;

            time = 0;
        }

        transitioning = true;
    }
}