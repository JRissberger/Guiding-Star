using UnityEngine;
using Unity.Cinemachine;
using Unity.Mathematics;    
using UnityEngine.Splines;

public class TrialCamera : MonoBehaviour
{
    [SerializeField] Vector3 cam3Displacement = new Vector3(11, 9, 0);
    CinemachineSplineDolly spline;
    public bool transitioning = false;
    public bool halfTrans = false;
    public int goal = 0;
    public float startPos;
    float time = 0;
    float timeToMove = 2f;


    void Start()
    {
        spline = this.GetComponent<CinemachineSplineDolly>();
    }

    void FixedUpdate()
    {
        if (transitioning)
        {
            time += Time.deltaTime * (1/timeToMove);
            float t = time * time / (2.0f * ((time * time) - time) + 1.0f);
            
            spline.CameraPosition = Mathf.Lerp(startPos, goal, t);

            if (time >= 0.5f)
                halfTrans = true;

            // If camera has reached the knot (finish transition)
            if (Mathf.Max(spline.CameraPosition, goal) - Mathf.Min(spline.CameraPosition, goal) <= 0.005) {
                transitioning = false;
                time = 0;
                spline.CameraPosition = goal;
                timeToMove = 2f;
                halfTrans = false;
            }
        }
    }

    /// <summary>
    /// Method that the triggers call to set the relevant camera variables and start its movement
    /// </summary>
    /// <param name="index">Whether the camera should be moving forwards or backwards (should only ever be -1 or 1)</param>
    /// <param name="t">The time it should take to transition (if less than zero, use default) </param>
    public void MoveTo(int index, float t)
    {
        startPos = spline.CameraPosition;

        // If the camera is mid-transition handle time and goal assignment
        if (transitioning) {
            timeToMove -= timeToMove * time;

            if (t >= 0) { timeToMove += t; }
            else { timeToMove += 2f; }

            if (index > 0)
                goal = (int)spline.CameraPosition + 2;
            else
                goal = (int)spline.CameraPosition - 1;
        }
        else
        {
            if (t >= 0)
                timeToMove = t;

            goal += index; 
        }

        time = 0;
        transitioning = true;
    }
}