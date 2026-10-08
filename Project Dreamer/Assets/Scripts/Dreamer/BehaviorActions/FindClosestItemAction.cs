using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "FindClosestItem", story: "Finds the closest [TargetItem] in [DreamerSight]", category: "Action", id: "4c590a6bf51ed4a1323f07158278e6b0")]
public partial class FindClosestItemAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> TargetItem;
    [SerializeReference] public BlackboardVariable<DreamerSight> DreamerSight;
    [SerializeReference] public BlackboardVariable<GameObject> Star;

    protected override Status OnStart()
    {
        //TODO: need to handle current held item. might be removed from visible list already?
        //Determine the closest item
        if (DreamerSight.Value.VisibleItems.Count > 0)
        {
            float distance = Mathf.Infinity;
            GameObject currentItem = DreamerSight.Value.VisibleItems[0];

            foreach (GameObject item in DreamerSight.Value.VisibleItems)
            {
                float tempDist = Vector3.Distance(Star.Value.transform.position, item.transform.position);
                if (tempDist < distance)
                {
                    distance = tempDist;
                    currentItem = item;
                }
            }

            //Update target item
            TargetItem.Value = currentItem;
            Debug.Log(TargetItem.Value);
        }
        else if (DreamerSight.Value.VisibleItems.Count > 0 && TargetItem.Value == null)
        {

            Debug.Log("hit find item node, but no items and target is null");
            
        }

        return Status.Success;
    }

}

