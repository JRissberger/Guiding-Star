using System;
using NUnit.Framework;
using Unity.Behavior;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using Unity.VisualScripting;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Sound Heard", story: "Sound heard or [IsInvestigating]? [SoundManager]", category: "Conditions", id: "0d54f36f5442cb9cf2faf000615d9f0f")]
public partial class SoundHeardCondition : Condition
{
    [SerializeReference] public BlackboardVariable<SoundManager> SoundManager;
    [SerializeReference] public BlackboardVariable<Boolean> IsInvestigating;

    public override bool IsTrue()
    {
        List<Sound> soundList = SoundManager.Value.HeardSounds;


        //Are there current sounds in hearing range, or is one currently being tracked?
        if (soundList.Count > 0 || IsInvestigating.Value)
        {
            //Sets to true for if there's sound but not a current investigation
            IsInvestigating.Value = true;
            return true;
        }


        //List is empty and no sound being tracked
        return false;
    }
}
