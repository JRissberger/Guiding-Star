using System;
using NUnit.Framework;
using Unity.Behavior;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using Unity.VisualScripting;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Soft Sound Heard", story: "Soft Sound heard or [IsInvestigating]? [SoundManager]", category: "Conditions", id: "0d54f36f5442cb9cf2faf000615d9f0f")]
public partial class SoftSoundHeardCondition : Condition
{
    [SerializeReference] public BlackboardVariable<SoundManager> SoundManager;
    [SerializeReference] public BlackboardVariable<Boolean> IsInvestigating;

    public override bool IsTrue()
    {
        List<Sound> soundList = SoundManager.Value.HeardSounds;

        //Is there currently a soft sound being checked?
        if (IsInvestigating.Value)
        {
            return true;
        }


        //Otherwise, loops through the sound list to see if any are soft
        foreach (Sound sound in soundList)
        {
            //Soft sound found
            if (sound.SoundType == SoundType.Soft)
            {
                IsInvestigating.Value = true;
                return true;
            }
        }

        //List is empty/no soft sounds and no sound being tracked
        return false;
    }
}
