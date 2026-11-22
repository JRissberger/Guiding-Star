using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "LoudSoundHeard", story: "Loud sound heard or [IsInvestigating] ? [SoundManager]", category: "Conditions", id: "3176f3471bc977ecbe685442afa0679b")]
public partial class LoudSoundHeardCondition : Condition
{
    [SerializeReference] public BlackboardVariable<SoundManager> SoundManager;
    [SerializeReference] public BlackboardVariable<Boolean> IsInvestigating;

    public override bool IsTrue()
    {
        List<Sound> soundList = SoundManager.Value.HeardSounds;


        //Is there currently a loud sound being checked?
        if (IsInvestigating.Value)
        {
            return true;
        }
        

        //Otherwise, loops through the sound list to see if any are loud
        foreach (Sound sound in soundList)
        {
            //Loud sound found
            if(sound.SoundType == SoundType.Loud)
            {
                IsInvestigating.Value = true;
                return true;
            }
        }

        //List is empty/no loud sounds and no sound being tracked
        return false;
    }
}
