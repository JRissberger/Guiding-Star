using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;
using Unity.VisualScripting;
using System.Runtime.CompilerServices;

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

    //Player reference for checking for held items
    private GameObject player;

    protected override Status OnStart()
    {
        //Getting player reference
        player = GameObject.Find("Player");

        navAgent = Star.Value.GetComponent<NavMeshAgent>();
        
        //Determine the closest item
        if (DreamerSight.Value.VisibleItems.Count > 0)
        {
            float distance = Mathf.Infinity;
            GameObject currentItem = null;
            
            //If there's already a target item, have it be the default to check against
            if (TargetItem.Value != null)
            { 
                currentItem = TargetItem.Value;
                distance = Vector3.Distance(Star.Value.transform.position, TargetItem.Value.transform.position);
            }

            //Loop through to find the best item to target
            foreach (GameObject item in DreamerSight.Value.VisibleItems)
            {
                float tempDist = Vector3.Distance(Star.Value.transform.position, item.transform.position);
                if (tempDist < distance && item != HeldItem.Value)
                {                   
                    //Check that a valid path exists and it isn't held by the player 
                    if (CanReach(item) && !HeldByPlayer(item))
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

            Debug.Log("checking");
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

    /// <summary>
    /// Checks if an item is currently being held by the player. Ignore it if so.
    /// This is checked here rather than sight since an item can still be "locked on" to after
    /// </summary>
    /// <param name="item"></param>     
    private bool HeldByPlayer(GameObject item)
    {
        if (player != null)
        {
            //Find all children of the player gameobject
            Transform[] children = player.GetComponentsInChildren<Transform>();

           
            //Loop through, and if there's one that matches the item, check if it's got the throwable script
                //Technically has the player in the list too, but since player isn't an item, a filter would be redundant
            foreach (Transform child in children)
            {
                if (child.gameObject == item)
                {
                    Debug.Log("Ignore this object!");
                    //Objects match, Star should ignore this object
                    return true;
                    
                }
            }
        }

        return false;
    }
}

