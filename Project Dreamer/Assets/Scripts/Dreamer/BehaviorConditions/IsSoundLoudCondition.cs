using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Is Sound Loud", story: "Is the [Sound] loud? Find [TargetPosition] if not", category: "Conditions", id: "ac1c4ddc709185b46f6b94ab4f6a5022")]
public partial class IsSoundLoudCondition : Condition
{
    [SerializeReference] public BlackboardVariable<LastHeardSound> Sound;
    [SerializeReference] public BlackboardVariable<Vector3> TargetPosition;

    private StarManager starManager;

    public override bool IsTrue()
    {
        if (Sound.Value.SoundType == SoundType.Loud)
        {
            //Get the Star manager and update state
            starManager = GameObject.GetComponent<StarManager>();
            starManager.UpdateStar(StarState.Scared);

            return true;
        }

        //Update target sound location, used to navigate to
        TargetPosition.Value = Sound.Value.Location;
        return false;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
