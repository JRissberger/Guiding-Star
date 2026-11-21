using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using System.Collections.Generic;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "FindWanderIndex", story: "Determine the best [WanderIndex] to target for wander", category: "Action", id: "d54e9e218f6d0ebd5698ce6d487a8273")]
public partial class FindWanderIndexAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Star;
    [SerializeReference] public BlackboardVariable<int> WanderIndex;
    [SerializeReference] public BlackboardVariable<MapSegmentManager> MapSegmentManager;

    protected override Status OnStart()
    {
        //Starting comparison data
        int bestIndex = -1;
        float closestDist = Mathf.Infinity;

        //Loop through all the map segments
        List<MapSegment> mapSegments = MapSegmentManager.Value.MapSegments;
        for (int i = 0; i < mapSegments.Count; i++)
        {
            //Skip if it's inactive
            if (!mapSegments[i].IsActive)
            {
                continue;
            }

            //Get direction to the point (might update to just check radius? getting this working first)
            Vector3 dirToPoint = mapSegments[i].CenterLoc - Star.Value.transform.position;

            //Nulling out height since that's not a factor NOTE: if we add verticality down the line it will become a factor!!
            dirToPoint.y = 0;
            Vector3 starForward = new Vector3(Star.Value.transform.forward.x, 0, Star.Value.transform.forward.z); //Also for star's forward, same reason

            //Dot product check, > 0 means it's in front, 0 means it's directly to the side, < 0 means it's behind
            float dot = Vector3.Dot(starForward, dirToPoint);

            //if the dot isn't above 1, skip over the index, it's not in view
            if (dot <= 0)
            {
                continue;
            }

            //Check distance and compare
            float newDistance = Mathf.Max(0f, Vector3.Distance(Star.Value.transform.position, mapSegments[i].CenterLoc) - mapSegments[i].Radius);
            if (newDistance < closestDist)
            {
                //Update data on best pick
                closestDist = newDistance;
                bestIndex = i;
            }
        }

        //Update wander index if a better pick was found
        if (bestIndex >= 0)
        {
            WanderIndex.Value = bestIndex;
            Debug.Log("Updating wander index to index " +  bestIndex);
        }

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

