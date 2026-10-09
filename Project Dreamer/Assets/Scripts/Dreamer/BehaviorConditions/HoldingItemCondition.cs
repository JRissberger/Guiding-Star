using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "HoldingItem", story: "Is Star [holding] item", category: "Variable Conditions", id: "ca0d8f27d6e35ed5d39e9a3bb7783c01")]
public partial class HoldingItemCondition : Condition
{
    [SerializeReference] public BlackboardVariable<bool> Holding;

    public override bool IsTrue()
    {
        return Holding.Value;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
