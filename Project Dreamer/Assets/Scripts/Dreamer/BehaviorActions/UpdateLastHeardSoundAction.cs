using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using System.Collections.Generic;
using Unity.VisualScripting;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Update Last Heard Sound", story: "Update [TargetSound]", category: "Action", id: "fe20db0aa2c468bac919aaa397934f8c")]
public partial class UpdateLastHeardSoundAction : Action
{
    [SerializeReference] public BlackboardVariable<LastHeardSound> TargetSound;
    [SerializeReference] public BlackboardVariable<LastHeardSound> NewTargetSound;

    protected override Status OnStart()
    {
        TargetSound.Value = NewTargetSound.Value;
        return Status.Success;
    }

}

