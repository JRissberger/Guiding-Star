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
    //Should update blackboard variables: Is Holding Item, and Current Held Item

    protected override Status OnStart()
    {
        //Access throwing script on Star
        DreamerThrowing throwing = Star.Value.GetComponent<DreamerThrowing>();

        if (throwing != null)
        {
            //Set held item as the item tied to blackboard
            throwing.SetHeldItem(Item.Value);
            HoldingItem.Value = true;
            HeldItem.Value = Item.Value;
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

