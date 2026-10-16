using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Random = UnityEngine.Random;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "NavigateJitter", story: "[Star] navigates to [target]", category: "Action", id: "34dc46e5c47b5597418808e635ede7a8")]
public partial class NavigateJitterAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Star;
    [SerializeReference] public BlackboardVariable<Vector3> Target;

    //"Binonmial distribution sample which provides the jitter, normally near 0 but with 
        //decreasing probability get closer to + or - maxrotation"
    private float rotationVar = 1f; //May adjust, eyeballed for testing
    private float maxRotation = 5f; //May adjust, pulled from Justus' code

    //Min/Max distance a new location can be set for
    private float minDistance = 0.2f;
    private float maxDistance = 1f;

    //Used for the partial destinations on the way to main target
    private Vector3 jitterTarget;

    //Navmesh agent for navigation
    private NavMeshAgent navAgent;

    //Used for handling manual turning
    private Quaternion manualRotate;

    protected override Status OnStart()
    {
        manualRotate = Quaternion.identity;
        navAgent = Star.Value.GetComponent<NavMeshAgent>();

        FindNextDestination();
        UpdateRotateVar();
        navAgent.SetDestination(jitterTarget);
        navAgent.autoBraking = false; //Disabled to make movement smoother

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        //TODO: breakout clause for if Star is stuck--off navmesh or abyss in all directions, return fail

        //Check distance to target, return running if not close enough
        if (!CloseToTarget(jitterTarget))
        {
            return Status.Running;
        }

        //If near target, check if target is close to final target
        if (CloseToTarget(Target.Value))
        {
            //Turn auto braking back on??
            return Status.Success;
        }


        //if not, reroll for new target
        //Always reroll at least once, but redo until a valid spot is found if needed
        //do {
        //    if (CheckForceTurn()) { return Status.Running; }
        //    FindNextDestination();

        //} while (!NavMesh.SamplePosition(new Vector3(jitterTarget.x, jitterTarget.y - Star.Value.transform.position.y, jitterTarget.z), out NavMeshHit hit, 0.1f, NavMesh.AllAreas));

        //DEBUG, do/while still results in infinite loop
        if (CheckForceTurn()) { return Status.Running; }
        FindNextDestination();
        //Finish movement update
        UpdateRotateVar();
        navAgent.SetDestination(jitterTarget);

        return Status.Running;
    }

    //TODO: weight turn towards overall target
    /// <summary>
    /// Forcibly turns Star if they're too close to a boundary
    /// </summary>
    /// <returns>if Star is currently rotating</returns>
    private bool CheckForceTurn()
    {
        //Find the closest edge to Star
        navAgent.FindClosestEdge(out NavMeshHit hit);

        //If edge is in range
        if (hit.distance < 0.5)
        {
            Debug.Log("Close to edge");
            //Calculating direction between Star and edge
            Vector3 direction = hit.position - Star.Value.transform.position;
            if (direction.sqrMagnitude > 0)
            {
                direction.Normalize();

                //Get dot to determine if Star's facing edge (closer to 1 means more perpendicular to edge)
                float dotFacing = Vector3.Dot(Star.Value.transform.forward, direction);

                //Roughly facing edge, need to turn
                if (dotFacing > 0.5f)
                {
                    //Set the target turn if there is none
                    if (manualRotate == Quaternion.identity)
                    {
                        manualRotate = Quaternion.Euler(0, 90f, 0) * Star.Value.transform.rotation;
                    }

                    //Rotate Star away           
                    Star.Value.transform.rotation = Quaternion.RotateTowards(Star.Value.transform.rotation, manualRotate, 180 * Time.deltaTime);
                    Debug.Log("turning");

                    //If Star has completed the turn, end
                    if (Quaternion.Angle(Star.Value.transform.rotation, manualRotate) < 1)
                    {
                        //Reset manual rotation
                        manualRotate = Quaternion.identity;
                    }

                    return true;
                }
            }
            
        }

        return false;
    }

    /// <summary>
    /// Determines a rotation amount to be added on to Star's current rotation
    /// </summary>
    /// <param name="rotation">Star's current rotation</param>
    /// <returns>Angle value to add on to the current rotation</returns>
    private float Rotate(float rotation)
    {
        Debug.Log("Rotate: " + (rotationVar - rotation) / 100f);
        if (Mathf.Abs(rotationVar - rotation) < 180)
        {
            return ((rotationVar - rotation) / 100f);
        }
        else
        {
            return -((rotationVar - rotation) / 100f);
        }
    }

    //TODO: likely to be adjusted to weight towards a destination
    /// <summary>
    /// Randomly updates the rotate variable by a small amount
    /// </summary>
    private void UpdateRotateVar()
    {
        rotationVar += ((Random.Range(0f, 100f) / 100f) - (Random.Range(0f, 100f)) * maxRotation);
        ClampRotation();
        //Debug.Log("New Rotate Var " + rotationVar);
    }

    /// <summary>
    /// Caps rotation within 360 degrees
    /// </summary>
    private void ClampRotation()
    {
        if (rotationVar > 360)
        {
            rotationVar -= 360;
        }
        else if (rotationVar < 0)
        {
            rotationVar += 360;
        }
    }

    /// <summary>
    /// Finds the next location for Star to move to on the way to the main target
    /// </summary>
    private void FindNextDestination()
    {
        //Calculate angle the new target is on
        float newAngle = Star.Value.transform.eulerAngles.y + Rotate(Star.Value.transform.eulerAngles.y);

        //Direction vector from new angle
        Vector3 direction = Quaternion.Euler(0f, newAngle, 0f) * Vector3.forward;

        //Update target location with random distance within max range
        jitterTarget = Star.Value.transform.position + direction * (Random.Range(minDistance, maxDistance));
        Debug.Log(jitterTarget);

        //TODO: use sampleposition to check if it's reachable and redo if not
    }

    /// <summary>
    /// Used to check if Star is close enough to a jitter wander target to reroll a new target position
    /// </summary>
    /// <returns>True if close enough, false otherwise</returns>
    private bool CloseToTarget(Vector3 target)
    {
        Vector3 distance = Star.Value.transform.position - target;
        if (distance.sqrMagnitude < 0.01f)
        {
            return true;
        }

        return false;
    }
}

