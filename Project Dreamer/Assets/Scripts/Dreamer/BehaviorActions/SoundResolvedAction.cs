using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Sound Resolved", story: "[Sound] investigation resolved", category: "Action", id: "2c6edb1725cfc7fbbcfb75afedc8ce92")]
public partial class SoundResolvedAction : Action
{

    [SerializeReference] public BlackboardVariable<LastHeardSound> Sound;

    protected override Status OnStart()
    {
        //Destroys the current scriptable object
        UnityEngine.Object.Destroy(Sound.Value);

        //Nulls sound value as an extra precaution
        Sound.Value = null;

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

