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
    private float rotationVar = 0.1f; //May adjust, eyeballed for testing
    private float maxRotation = 5; //May adjust, pulled from Justus' code

    //Min/Max distance a new location can be set for
    private float minDistance = 1f;
    private float maxDistance = 2f;

    //Used for the partial destinations on the way to main target
    private Vector3 jitterTarget;

    //Navmesh agent for navigation
    private NavMeshAgent navAgent;

    protected override Status OnStart()
    {
        navAgent = Star.Value.GetComponent<NavMeshAgent>();

        FindNextDestination();
        UpdateRotateVar();
        navAgent.SetDestination(jitterTarget); //TODO: Speed?

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        //Check distance to target, return running if not close enough
        if (!CloseToTarget(jitterTarget))
        {
            return Status.Running;
        }

        //If near target, check if target is close to final target
        if (CloseToTarget(Target.Value))
        {
            return Status.Success;
        }

        //if not, reroll for new target
        //DEBUG: perpetually rerolling (unless randomly hits target), this is full random right now
        FindNextDestination();
        UpdateRotateVar();
        navAgent.SetDestination(jitterTarget);
        return Status.Running;
    }

    /// <summary>
    /// Determines a rotation amount to be added on to Star's current rotation
    /// </summary>
    /// <param name="rotation">Star's current rotation</param>
    /// <returns>Angle value to add on to the current rotation</returns>
    private float Rotate(float rotation)
    {
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
    }

    /// <summary>
    /// Finds the next location for Star to move to on the way to the main target
    /// </summary>
    private void FindNextDestination()
    {
        //Calculate angle the new target is on
        float newAngle = Star.Value.transform.eulerAngles.y + Rotate(rotationVar);

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

