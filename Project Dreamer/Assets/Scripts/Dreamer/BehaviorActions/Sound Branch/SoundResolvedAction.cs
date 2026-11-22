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
    [SerializeReference] public BlackboardVariable<Boolean> IsInvestigatingLoud;

    [SerializeReference] public BlackboardVariable<Boolean> IsInvestigatingSoft;

    [SerializeReference] public BlackboardVariable<Vector3> TargetPosition;

    private StarManager starManager;

    protected override Status OnStart()
    {
        //Updates previous sound and nulls target sound
        UnityEngine.Object.Destroy(TargetSound.Value);
        //UnityEngine.Object.Destroy(NewTargetSound.Value);
        TargetSound.Value = null;
        //NewTargetSound.Value = null;
        IsInvestigatingLoud.Value = false;
        IsInvestigatingSoft.Value = false;

        //Also resetting target position to navigate to
        TargetPosition.Value = Vector3.zero;

        //Get the Star manager and update state to default
        starManager = GameObject.GetComponent<StarManager>();
        starManager.UpdateStar(StarState.Wander);

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

