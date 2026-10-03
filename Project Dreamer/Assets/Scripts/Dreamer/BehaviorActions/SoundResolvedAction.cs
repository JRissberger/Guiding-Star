using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Sound Resolved", story: "[TargetSound] investigation resolved", category: "Action", id: "2c6edb1725cfc7fbbcfb75afedc8ce92")]
public partial class SoundResolvedAction : Action
{

    [SerializeReference] public BlackboardVariable<LastHeardSound> TargetSound;
    [SerializeReference] public BlackboardVariable<LastHeardSound> NewTargetSound;
    [SerializeReference] public BlackboardVariable<Boolean> IsInvestigating;

    protected override Status OnStart()
    {
        //Updates previous sound and nulls target sound

        Debug.Log("resolving sounds");
        UnityEngine.Object.Destroy(TargetSound.Value);
        UnityEngine.Object.Destroy(NewTargetSound.Value);
        TargetSound.Value = null;
        NewTargetSound.Value = null;
        IsInvestigating.Value = false;
        
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

