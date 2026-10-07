using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "ItemInLOS", story: "Item in LOS? Update [TargetItem]", category: "Conditions", id: "b2f3bfcdb8213e2e252343e8acb031b0")]
public partial class ItemInLosCondition : Condition
{
    //Receive on blackboard: list of items in los. Should be passed in from external sight script...vision cone in front of star
    //Modifying on blackboard: target item
    [SerializeReference] public BlackboardVariable<GameObject> TargetItem; 


    public override bool IsTrue()
    {
        //Determine closest item, set as target
        //Make sure to discount current held item

        return true;

        //If no iteminlos: return false, node doesn't go

        //Item in los: Determine item to prioritize off list (may need a type hierarchy. would be type indicator on object. otherwise prio closest)
            //Set target object, target position, return true
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
