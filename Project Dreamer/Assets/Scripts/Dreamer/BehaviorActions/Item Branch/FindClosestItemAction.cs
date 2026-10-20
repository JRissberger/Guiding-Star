using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;
using Unity.VisualScripting;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "FindClosestItem", story: "Finds the closest [TargetItem] in [DreamerSight]", category: "Action", id: "4c590a6bf51ed4a1323f07158278e6b0")]
public partial class FindClosestItemAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> TargetItem;
    [SerializeReference] public BlackboardVariable<DreamerSight> DreamerSight;
    [SerializeReference] public BlackboardVariable<GameObject> Star;
    [SerializeReference] public BlackboardVariable<GameObject> HeldItem;

    //Used to check for valid paths
    private NavMeshAgent navAgent;

    protected override Status OnStart()
    {
        navAgent = Star.Value.GetComponent<NavMeshAgent>();
        
        //Determine the closest item
        if (DreamerSight.Value.VisibleItems.Count > 0)
        {
            float distance = Mathf.Infinity;
            GameObject currentItem = null;            

            //Loop through to find the best item to target
            foreach (GameObject item in DreamerSight.Value.VisibleItems)
            {
                float tempDist = Vector3.Distance(Star.Value.transform.position, item.transform.position);
                if (tempDist < distance && item != HeldItem.Value)
                {                   
                    //Check that a valid path exists
                    if (CanReach(item))
                    {
                        //Path is valid, can make item the target
                        distance = tempDist;
                        currentItem = item;
                    }                   
                }
            }

            //If item is still null, there's nothing that can be pathed to, end the node
            if (!currentItem)
            {
                Debug.LogWarning("Seeing items but none can be pathed to");
                return Status.Failure;
            }

            //Update target item
            TargetItem.Value = currentItem;
        }

        return Status.Success;
    }

    /// <summary>
    /// Checks if an item can be reached on the navmesh
    /// </summary>
    /// <param name="item">The item object to check the position of</param>
    /// <returns>True if there's a complete path Star can use</returns>
    private bool CanReach(GameObject item)
    {
        //Get the path
        NavMeshPath path = new NavMeshPath();
        navAgent.CalculatePath(item.transform.position, path);

        //Check that a valid path exists
        if (path.status == NavMeshPathStatus.PathComplete)
        {
            return true;
        }

        //DEBUG, remove later
        else
        {
            Debug.Log("Item found but no valid path: " + item);
            return false;
        }
    }

}

