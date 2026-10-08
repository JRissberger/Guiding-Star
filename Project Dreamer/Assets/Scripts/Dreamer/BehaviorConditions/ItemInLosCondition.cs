using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "ItemInLOS", story: "Item in [DreamerSight] or [SawItem] already? [HeldItem]", category: "Conditions", id: "b2f3bfcdb8213e2e252343e8acb031b0")]
public partial class ItemInLosCondition : Condition
{
    
    [SerializeReference] public BlackboardVariable<DreamerSight> DreamerSight;
    [SerializeReference] public BlackboardVariable<Boolean> SawItem;
    [SerializeReference] public BlackboardVariable<GameObject> HeldItem;


    public override bool IsTrue()
    {
        /* Edge case of the one visible item being the currently held item,
         * with no previous target to investigate
         */
        if (DreamerSight.Value.VisibleItems.Count == 1 && !SawItem.Value && DreamerSight.Value.VisibleItems[0] == HeldItem.Value)
        {
            Debug.Log("Edge case hit, ignoring " + DreamerSight.Value.VisibleItems[0].gameObject);
            return false;
        }

        if (DreamerSight.Value.VisibleItems.Count > 0 || SawItem.Value)
        {
            
            SawItem.Value = true;
            return true;
        }
        

        return false;
    }
}
