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

    //Amount used to offset Star's rotation by
    private float rotationVar = 0f; //May adjust, eyeballed for testing
    private float maxRotation = 5f; //May adjust, pulled from Justus' code

    //Min/Max distance a new location can be set for
    private float minDistance = 0.2f;
    private float maxDistance = 1f;

    //Used for the partial destinations on the way to main target
    private Vector3 jitterTarget;

    //Navmesh agent for navigation
    private NavMeshAgent navAgent;

    //Used when calculating the rotation to make the move change more gradual. Lower number is faster turn
    float rotationDivisor = 10f;


    protected override Status OnStart()
    {
        //Starts equal to Star's rotation
        rotationVar = Star.Value.transform.eulerAngles.y;
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

        //DEBUG, do/while still results in infinite loop
        
        FindNextDestination();

        //Finish movement update
        UpdateRotateVar();
        navAgent.SetDestination(jitterTarget);
        Debug.Log(rotationVar);
        return Status.Running;
    }

    //TODO: weight turn towards overall target
        //Might temporarily update turn rate? Set a timer and put it back when done
    /// <summary>
    /// Forcibly turns Star if they're too close to a boundary
    /// </summary>
    /// <returns>if Star is currently rotating</returns>
    private bool CheckForceTurn()
    {
        //Find the closest edge to Star
        navAgent.FindClosestEdge(out NavMeshHit hit);

        //If edge is in range
        if (hit.distance < 1)
        {
            //Calculating direction between Star and edge
            Vector3 direction = hit.position - Star.Value.transform.position;
            direction.y = 0;
            
            if (direction.sqrMagnitude > 0)
            {
                direction.Normalize();

                //Get dot to determine if Star's facing edge (closer to 1 means more perpendicular to edge)
                Vector3 facing = Star.Value.transform.forward;
                facing.y = 0; //NOTE: set to 0 for now, will update if Star can move vertically in levels
                facing.Normalize();
                float dotFacing = Vector3.Dot(facing, direction);

                //Roughly facing edge, need to turn
                if (dotFacing > 0.5f)
                {
                    //Get direction away from edge
                    Vector3 away = -direction;

                    //Get random angle in range for turning to pick new direction
                        //TODO: this should be weighted towards the eventual target
                    float angle = Random.Range(-60, 60);
                    Vector3 randomDir = Quaternion.Euler(0, angle, 0) * away;

                    //Update rotate var to make Star move away
                    rotationVar = Mathf.Repeat(Quaternion.LookRotation(randomDir).eulerAngles.y, 360f);

                    Debug.Log("Move away from edge! New rotation: " + rotationVar);
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
        //Using deltaangle to find shortest diff instead of calculating manually
        float diff = (Mathf.DeltaAngle(rotation, rotationVar)) / rotationDivisor;
        Debug.Log("Rotate amount: " + diff);
        return diff;
    }

    //TODO: likely to be adjusted to weight towards a destination
    /// <summary>
    /// Randomly updates the rotate variable by a small amount
    /// </summary>
    private void UpdateRotateVar()
    {
        rotationVar += ((Random.Range(0f, 100f) / rotationDivisor) - (Random.Range(0f, 100f) / rotationDivisor)) * maxRotation;

        //Keeps rotation within 360 degrees, rolls over
        Mathf.Repeat(rotationVar, 360); 

        Debug.Log("Rotation offset: " +  rotationVar);
    }

    /// <summary>
    /// Finds the next location for Star to move to on the way to the main target
    /// </summary>
    private void FindNextDestination()
    {
        //Force move away from edge if needed 
        if (CheckForceTurn()) { return; }

        //Calculate angle the new target is on
        float newAngle = Star.Value.transform.eulerAngles.y + Rotate(Star.Value.transform.eulerAngles.y);

        //Direction vector from new angle
        Vector3 direction = Quaternion.Euler(0f, newAngle, 0f) * Vector3.forward;

        //Update target location with random distance within max range
        jitterTarget = Star.Value.transform.position + direction * (Random.Range(minDistance, maxDistance));
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

