using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "SeesMemory", story: "Memory in [view] ? Make it [TargetItem]", category: "Conditions", id: "c734e107f874af025b3c909e96516148")]
public partial class SeesMemoryCondition : Condition
{
    [SerializeReference] public BlackboardVariable<DreamerSight> View;
    [SerializeReference] public BlackboardVariable<GameObject> TargetItem;

    public override bool IsTrue()
    {
        //Checks if there's anything tagged as a memory in view
            //NOTE: shouldn't ever have a point where there's two memories in view,
            //but if needed can evaluate which is closer
        foreach(GameObject item in View.Value.VisibleItems)
        {
            if (item.CompareTag("Memory"))
            {
                //Set item and return
                TargetItem.Value = item;
                return true;
            }
        }

        return false;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
