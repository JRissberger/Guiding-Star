using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Random = UnityEngine.Random;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "NavigateJitter", story: "[Star] navigates to [target] or [item]", category: "Action", id: "34dc46e5c47b5597418808e635ede7a8")]
public partial class NavigateJitterAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Star;
    [SerializeReference] public BlackboardVariable<Vector3> Target;

    //Adding item here for if Star should target an item instead
    [SerializeReference] public BlackboardVariable<GameObject> Item;

    //Select this in editor to target item
    [SerializeReference] public BlackboardVariable<bool> targetItem;

    //Amount used to offset Star's rotation by
    private float rotationVar = 0f; 
    private float maxRotation = 10f; 

    //Min/Max distance a new location can be set for
    private float minDistance = 0.1f;
    private float maxDistance = 0.5f;

    //Distance to weight nav more heavily to target
    private float approachDistance = 2f;
    private float directDistance = 1f;

    //Used for the partial destinations on the way to main target
    private Vector3 jitterTarget;

    //Navmesh agent for navigation
    private NavMeshAgent navAgent;

    //Used when calculating the rotation to make the move change more gradual. Lower number is faster turn
    [SerializeReference] public BlackboardVariable<float> rotationDivisor;

    //Weight to overall target
    [SerializeReference] public BlackboardVariable<float> targetNavWeight;
    float closingWeight = 0f; //Increased as Star approaches, reset at start each time to match og

    //Value to increase/decrease speed by
    private float speedChange = 0.5f;
    [SerializeReference] public BlackboardVariable<float> baseSpeed;


    protected override Status OnStart()
    {
        //Starts equal to Star's rotation
        rotationVar = Star.Value.transform.eulerAngles.y;
        navAgent = Star.Value.GetComponent<NavMeshAgent>();

        //Reset navweight
        closingWeight = targetNavWeight;

        //Should Star be targeting an item instead?
        //If the target loc is default and there's an item added, use the item
        if (targetItem)
        {
            //Set the target location to be the item's location
            Target.Value = Item.Value.transform.position;
        }


        if (!FindNextDestination())
        {
            return Status.Failure;
        }

        EnsureLocationOnMesh(jitterTarget);
        navAgent.SetDestination(jitterTarget);

        navAgent.autoBraking = false; //Disabled to make movement smoother

        //Ensure there's a valid way to get to the final target
        if (!ValidatePath(Target.Value))
        {
            //Node fails, shouldn't get caught in infinite failure as other precautions above navigation will move Star to a more default branch. 
            return Status.Failure;
        }

       
        return Status.Running;
    }

    protected override Status OnUpdate()
    {

        //If near target, check if target is close to final target
        if (CloseToTarget(Target.Value))
        {
            return Status.Success;
        }


        //Check distance to target, return running if not close enough
        if (!CloseToTarget(jitterTarget))
        {            
            return Status.Running;
        }

        //Finds the next jitter target. If no valid target can be found after multiple attempts, this node fails
            //Used to prevent potential infinite loop situations
        if (!FindNextDestination())
        {
            return Status.Failure;
        }

        EnsureLocationOnMesh(jitterTarget);

        //If for some reason the final target is no longer accessible, end
            //Done each update loop to account for items potentially moving, or paths being blocked
        if (!ValidatePath(Target.Value))
        {
            return Status.Failure;
        }

        navAgent.SetDestination(jitterTarget);

        //Slightly change speed
        navAgent.speed = baseSpeed + Random.Range(-speedChange, speedChange);

        return Status.Running;
    }

    //TODO: weight turn towards overall target
        //NOTE: may rework for efficiency, weighting towards target. may also be unneeded due to target weighting
        //Could do "whiskers" (volume on either side of Star) that handles edge--move until theyre further away
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
                    Vector3 randomDir = Quaternion.Euler(0, Random.Range(-5, 5), 0) * away;
                   

                    //Update target directly since weighting towards rotationVar takes too long
                    jitterTarget = Star.Value.transform.position + randomDir.normalized * (Random.Range(minDistance, maxDistance));

                    //Updates rotationVar to the new angle so Star doesn't move in loops
                    rotationVar = Quaternion.LookRotation(randomDir).eulerAngles.y;

                    Debug.Log("Moving from edge to: " + jitterTarget);
                    Debug.Log("New rotation var: " + rotationVar);
                        
                    
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
        return diff;
    }

    /// <summary>
    /// Randomly updates the rotate variable by a small amount
    /// </summary>
    private void UpdateRotateVar()
    {
        rotationVar += ((Random.Range(0f, 100f) / 100f) - (Random.Range(0f, 100f) / 100f)) * maxRotation;

        //Keeps rotation within 360 degrees, rolls over
        rotationVar = Mathf.Repeat(rotationVar, 360); 
    }

    /// <summary>
    /// Finds the next location for Star to move to on the way to the main target
    /// </summary>
    private bool FindNextDestination()
    {
        //Check if Star is very close to the final target
        Vector3 distToFinal = Target.Value - Star.Value.transform.position;
        distToFinal.y = 0;
               
        //If true, close, set target directly and end method early
        if (distToFinal.magnitude <= directDistance)
        {
            //Turn auto braking back on for smoother stop
            navAgent.autoBraking = true;
            jitterTarget = new Vector3(Target.Value.x, Star.Value.transform.position.y, Target.Value.z);
            return true; 
        }

        //Used to handle potential bad location rolls
        bool validPosition = false;
        int attemptCount = 0; //Used to avoid infinite loop with rerolling

        while (!validPosition)
        {
            //Continue wandering, with new rotation
            UpdateRotateVar();

            //Calculate angle the new target is on
            float newAngle = Star.Value.transform.eulerAngles.y + Rotate(Star.Value.transform.eulerAngles.y);

            //Direction vector from new angle
            Vector3 direction = Quaternion.Euler(0f, newAngle, 0f) * Vector3.forward;

            //Overall target direction
            Vector3 targetDirection = Target.Value - Star.Value.transform.position;

            //Weight more heavily to target when close
            if (distToFinal.magnitude < approachDistance)
            {
                closingWeight = Mathf.Lerp(1f, closingWeight, distToFinal.magnitude / approachDistance);
            }

            //Using lerp to blend the wander target and overall target
            Vector3 overallDirection = Vector3.Slerp(direction, targetDirection, targetNavWeight);
            Vector3 overallDirNormalized = overallDirection.normalized;

            //Update target location with random distance within max range
            jitterTarget = Star.Value.transform.position + overallDirNormalized * (Random.Range(minDistance, maxDistance));

            //Check for validity, reroll otherwise
                //NOTE: might need to keep an eye out for potential race conditions with checking loc validity and path, if loc gets updated
            if (EnsureLocationOnMesh(jitterTarget) && ValidatePath(jitterTarget))
            {
                validPosition = true;
            }
            
            //Increments attempts. If Star keeps failing to path, end the method with false.
                //Realistically shouldn't happen as targets are validated ahead of time
            attemptCount++;
            if (attemptCount >= 10)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Checks that target is on mesh and updates to nearest point on mesh if needed
    /// Important: Separate from ValidatePath. Only checks that location is on mesh, not that it can be pathed to
    /// </summary>
    /// <param name="location">Location being checked</param>
    private bool EnsureLocationOnMesh(Vector3 location)
    {
        //NOTE: may be tweaked as needed for accuracy, especially with any vertical movement that might be added later        
        //TODO: check absolute pathing from agent pos?
        location.y = 0f; //NOTE: squashing y for the time being but similarly, may need to be tweaked for vertical traversal
        if (NavMesh.SamplePosition(location, out NavMeshHit hit, 1f, NavMesh.AllAreas))
        {
            //Update target but keeps y value the same (should be the same value if the target is already on the mesh)
                //NOTE: may need to check more if vertical traversal is added, but works for the time being
            jitterTarget = new Vector3(hit.position.x, Star.Value.transform.position.y, hit.position.z);
            return true;
        }

        //Will only be hit if the position is nowhere near the navmesh, which realistically shouldn't occur
        else
        {
            Debug.LogWarning("No targets found near position " + location);
            return false;
        }
    }

    /// <summary>
    /// Used to check if Star is close enough to a jitter wander target to reroll a new target position
    /// </summary>
    /// <returns>True if close enough, false otherwise</returns>
    private bool CloseToTarget(Vector3 target)
    {
        Vector3 distance = Star.Value.transform.position - target;

        //NOTE: this will need to be updated for if we have height variation--get navmesh height? star height? will look into having this more scalable
            //Will work if star and navmesh are on same level, assuming I can get the nav y position, but could evaluate poorly if Star is UNDER an object or above
            //Unless top of objects is considered a different navmesh? things to consider and look into as we progress
        distance.y = 0;
        
        if (distance.sqrMagnitude < 0.01f)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Checks that the current targeted location can be pathed to on the mesh
    /// </summary>
    /// <param name="target">Location to check</param>
    /// <returns>True if it can be pathed to</returns>
    private bool ValidatePath(Vector3 target)
    {
        NavMeshPath path = new NavMeshPath();
        navAgent.CalculatePath(target, path);
        if (path.status == NavMeshPathStatus.PathComplete)
        {
            return true;
        }

        return false;
    }
}

