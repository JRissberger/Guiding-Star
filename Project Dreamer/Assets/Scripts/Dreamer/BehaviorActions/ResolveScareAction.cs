using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ResolveScare", story: "Resolve [Scare] response", category: "Action", id: "78bd91e11719cf68a3937890bf8d0b48")]
public partial class ResolveScareAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Scare;
    [SerializeReference] public BlackboardVariable<bool> sawScare;

    protected override Status OnStart()
    {
        Debug.LogWarning("resolved scare");
        Scare.Value = null;
        sawScare.Value = false;
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

