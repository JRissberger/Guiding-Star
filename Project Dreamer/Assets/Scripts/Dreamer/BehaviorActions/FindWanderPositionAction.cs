using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

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
        Collider[] colliders = Physics.OverlapSphere(currentSegment.CenterLoc, currentSegment.Radius, ~LayerMask.GetMask("Star", "DreamerCompoundSight"));
        activeLights = new List<Collider>();
        inactiveLights = new List<Collider>();

        //Loop through to identify and sort any lights
        foreach (Collider collider in colliders)
        {
            //Try to see if the collider's gameobject is a light
            if (collider.gameObject.TryGetComponent<LightHazard>(out LightHazard lightScript))
            {
                //Add it to the corresponding list
                if (lightScript.IsActive)
                {
                    activeLights.Add(collider);
                }
                else
                {
                    inactiveLights.Add(collider);
                }
            }
        }

        
        //Check to see if the circle is fully covered by disabled lights
        if (inactiveLights.Count > 0 && IsFullyCovered(currentSegment.CenterLoc, currentSegment.Radius, inactiveLights))
        {
            //Nowhere for Star to move, skip over the point
            Debug.LogWarning("Wander point is likely fully inaccessible!");

            //Increment index 
            WanderIndex.Value++;

            //Incrementing the index but returning failure means that a re attempt will move down the wander list
            return Status.Failure;
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
                        inActiveLight = true;
                        break;
                    }
                }

                //Final check for valid position
                if (!inInactiveLight && (inActiveLight || activeLights.Count == 0)) //Account for there being no lights
                {
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

    /// <summary>
    /// Determines if a wander area is fully covered by lights in the provided list. 
    /// Divides the circle into a grid of points and runs a check on each to see if there's collision with a light.
    /// Technically speaking this isn't perfect but should be good enough for wander pathing and can be updated with more sample points if we really need it
    /// </summary>
    /// <param name="center">Center of the circle</param>
    /// <param name="radius">Radius of the circle</param>
    /// <param name="lights">List of lights to check. Should be a list of inactive or active lights, not a mix</param>
    /// <returns>True if the area is fully covered</returns>
    private bool IsFullyCovered(Vector3 center, float radius, List<Collider> lights)
    {
        //How many points along each axis are being used to make the grid
            //Using 10 is kinda arbitrary and we can change it for more precision if needed
        int scale = 10;

        //How many points are going to be used
            //This is getting the length of the circle and cutting it up into a number of points
        float point = (radius * 2f) / scale;

        //Loop through each point
        for (int x = 0; x < scale; x++)
        {
            //Using z since the circle is flat on the ground
            for (int z = 0; z < scale; z++)
            {
                //Calculating the coordinates for the point
                    //Moving left to right on an axis starting from the edge, adding the number point we're on
                float xCoord = -radius + x * point;
                float zCoord = -radius + z * point;

                //Adding on the center location to the calculated loc
                Vector3 testPoint = center + new Vector3(xCoord, 0, zCoord);
                
                //Skipping the point if it's outside the circle (since technically this makes a square)
                if (new Vector2(xCoord, zCoord).sqrMagnitude > radius * radius) { continue; }

                //Check for a light covering the point
                    //Doesn't have to be all of them
                bool covered = false;
                foreach (Collider collider in lights)
                {
                    if (IsInsideLight(testPoint, collider))
                    {
                        covered = true;
                        break; //Only need one light to cover so can skip the rest for this point
                    }
                }

                //None of the lights overlap the point
                if (!covered)
                {
                    //Stops check here since there's at least one open spot
                    return false;
                }
            }
        }

        return true;
    }
}

