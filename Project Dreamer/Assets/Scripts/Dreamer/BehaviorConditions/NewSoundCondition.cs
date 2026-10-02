using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "NewSound", story: "Is [TargetSound] different from [PrevSound]", category: "Conditions", id: "d546e7b7cd323c362dc224f8fba5d19a")]
public partial class NewSoundCondition : Condition
{
    [SerializeReference] public BlackboardVariable<LastHeardSound> TargetSound;
    [SerializeReference] public BlackboardVariable<LastHeardSound> PrevSound;

    public override bool IsTrue()
    {
        if (TargetSound.Value == PrevSound.Value)
        {
            Debug.Log("Aborting branch, sound updated");
            return true;
        }

        return false;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
