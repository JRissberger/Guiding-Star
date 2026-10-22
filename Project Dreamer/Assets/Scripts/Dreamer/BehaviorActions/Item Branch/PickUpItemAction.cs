using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Pick Up Item", story: "[Star] picks up [item]", category: "Action", id: "7fc043eb87c6000d98adb188f0f65f94")]
public partial class PickUpItemAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Star;
    [SerializeReference] public BlackboardVariable<GameObject> Item;
    [SerializeReference] public BlackboardVariable<GameObject> HeldItem;
    [SerializeReference] public BlackboardVariable<Boolean> HoldingItem;
    [SerializeReference] public BlackboardVariable<float> PickupRange;

    protected override Status OnStart()
    {
        //Access throwing script on Star
        DreamerThrowing throwing = Star.Value.GetComponent<DreamerThrowing>();

        //Distance between Star and item
        float distance = Vector3.Distance(Star.Value.transform.position, Item.Value.transform.position);

        //Only pick up if item is in range.
        if (throwing != null && distance < PickupRange.Value)
        {
            //Null out any velocity on the rigidbody so the item doesn't move oddly during pickup
                //This is mostly for accounting for if an item is picked up when it's still falling, letting Star "catch" items if they're close enough
            Rigidbody rigidbody = Item.Value.GetComponent<Rigidbody>();
            rigidbody.angularVelocity = Vector3.zero;
            rigidbody.linearVelocity = Vector3.zero;

            //Set held item as the item tied to blackboard
            throwing.SetHeldItem(Item.Value);
            HoldingItem.Value = true;
            HeldItem.Value = Item.Value;
            return Status.Success;
        }

        return Status.Failure;
    }

}

