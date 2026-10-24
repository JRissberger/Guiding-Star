using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;
using System.Collections.Generic;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "FindWanderPosition", story: "[Star] finds a [wanderPosition] in the current map segment.", category: "Action", id: "f210b7b0cc9651b9682353065a084e8a")]
public partial class FindWanderPositionAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Star;
    [SerializeReference] public BlackboardVariable<Vector3> WanderPosition;
    [SerializeReference] public BlackboardVariable<int> WanderIndex;
    [SerializeReference] public BlackboardVariable<MapSegmentManager> MapSegmentManager;

    //Lists for active and inactive light colliders
        //TODO: Should be updated to final script name later
    private List<Collider> activeLights;
    private List<Collider> inactiveLights;

    protected override Status OnStart()
    {
        //Looks for valid position, possible retry since map segments are circles, so some parts may overlap with walls
        bool foundValidTarget = false;
        Vector3 position = Vector3.zero;

        //Get the current map segment according to the wander index
        MapSegment currentSegment = MapSegmentManager.Value.MapSegments[WanderIndex.Value];

        //Check to see if there's any colliders in this area
            //TODO: Having lights on a specified layer could make this easier since there's a layer mask overload. Look into?
        Collider[] colliders = Physics.OverlapSphere(currentSegment.CenterLoc, currentSegment.Radius);
        activeLights = new List<Collider>();
        inactiveLights = new List<Collider>();

        //Loop through to identify and sort any lights
        foreach (Collider collider in colliders)
        {
            //Try to see if the collider's gameobject is a light
                //TODO: update script search to look for finalized script
            if (collider.gameObject.TryGetComponent<TEMPLight>(out TEMPLight lightScript))
            {
                //Add it to the corresponding list
                    //TODO: update variable name to final
                if (lightScript.isActive)
                {
                    activeLights.Add(collider);
                }
                else
                {
                    inactiveLights.Add(collider);
                }
            }
        }

        while (!foundValidTarget)
        {
            //Find a random position in the current segment's boundaries
            position = currentSegment.CenterLoc + UnityEngine.Random.insideUnitSphere * currentSegment.Radius;

            //Is the position on the navmesh
            if (NavMesh.SamplePosition(position, out NavMeshHit hit, currentSegment.Radius, NavMesh.AllAreas))
            {
                //Using bools to track since break can't be used in inner loops to break outer loops
                bool inInactiveLight = false;
                bool inActiveLight = false;

                //Check to make sure it's not in any inactive lights
                //TODO: again, change script name
                foreach (Collider light in inactiveLights)
                {
                    if (IsInsideLight(position, light))
                    {
                        inInactiveLight = true;
                        break;
                    }
                }

                //If there's any active lights, the position should be in one
                foreach (Collider light in activeLights)
                {
                    if (IsInsideLight(position, light))
                    {
                        Debug.Log("Position inside light!");
                        inActiveLight = true;
                        break;
                    }
                }

                //Final check for valid position
                if (!inInactiveLight && (inActiveLight || activeLights.Count == 0)) //Account for there being no lights
                {
                    Debug.Log("Valid target: " + position);
                    foundValidTarget = true;
                }
            }
        }

        //Update blackboard variable
        WanderPosition.Value = position;
        
        return Status.Success;
    }

    /// <summary>
    /// Checks if a given position is within a light object
    /// </summary>
    /// <param name="position">Position to check</param>
    /// <param name="light">Light object to check</param>
    /// <returns>True if the position is in the light</returns>
    private bool IsInsideLight(Vector3 position, Collider light)
    {
        //Gets the closest point on the collider to the given position
        Vector3 closePoint = light.ClosestPoint(position);

        //Difference should be very small/zero for any point inside the collider
        if (Vector3.Distance(closePoint, position) < 0.01f)
        {
            return true;
        }

        return false;
    }
}

