using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Sound Resolved", story: "[Sound] investigation resolved", category: "Action", id: "2c6edb1725cfc7fbbcfb75afedc8ce92")]
public partial class SoundResolvedAction : Action
{

    [SerializeReference] public BlackboardVariable<LastHeardSound> TargetSound;
    [SerializeReference] public BlackboardVariable<LastHeardSound> PrevTargetSound;

    protected override Status OnStart()
    {
        //Updates previous sound and nulls target sound
        PrevTargetSound.Value = TargetSound.Value;
        TargetSound.Value = null;

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

