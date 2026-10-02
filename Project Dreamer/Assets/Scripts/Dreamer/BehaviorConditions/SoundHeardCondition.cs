using System;
using NUnit.Framework;
using Unity.Behavior;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using Unity.VisualScripting;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Sound Heard", story: "Sound heard? [Star] [SoundManager] [LastHeardSound]", category: "Conditions", id: "0d54f36f5442cb9cf2faf000615d9f0f")]
public partial class SoundHeardCondition : Condition
{
    [SerializeReference] public BlackboardVariable<SoundManager> SoundManager;
    [SerializeReference] public BlackboardVariable<LastHeardSound> LastHeardSound;

    public override bool IsTrue()
    {
        List<Sound> soundList = SoundManager.Value.HeardSounds;

        //Are there current sounds in hearing range, or is one currently being tracked?
        if (soundList.Count > 0 || LastHeardSound.Value)
        {            
            //DEBUG
            if (LastHeardSound.Value)
            {
                Debug.Log("Tracking old sound");
            }

            return true;
        }

        //List is empty and no sound being tracked
        return false;
    }
}
