using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "ScareInLOS", story: "Scare in LOS ?", category: "Conditions", id: "3a30d06eeb8cc633549fc83d5e2ebf53")]
public partial class ScareInLosCondition : Condition
{

    public override bool IsTrue()
    {
        return true;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
