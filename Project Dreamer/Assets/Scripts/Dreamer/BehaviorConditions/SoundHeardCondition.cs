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
    [SerializeReference] public BlackboardVariable<GameObject> Star;
    [SerializeReference] public BlackboardVariable<LastHeardSound> LastHeardSound;

    public override bool IsTrue()
    {
        List<Sound> soundList = SoundManager.Value.HeardSounds;

        //Are there current sounds in hearing range?
        if (soundList.Count > 0)
        {

            Sound closestSound = soundList[0];
            float distance = Mathf.Infinity;

            //Compare sounds, find the shortest distance
            for (int i = 0; i < soundList.Count; i++)
            {
                //Get the distance between the current sound and Star
                float currDistance = Vector3.Distance(Star.Value.transform.position, soundList[i].gameObject.transform.position);

                //If it's less than the current min, update
                if (currDistance < distance)
                {
                    distance = currDistance;
                    closestSound = soundList[i];
                }
            }

            //Create last heard sound scriptable object, set it to value
            /* IMPORTANT: Scriptable objects aren't gotten by the garbage collector.
             * They need to be deleted manually otherwise it'll eventually cause a memory leak.
             * Current solution is if there's an existing one that doesn't match the data, destroy the old one
             * If there's a memory issue with the project, it probably is from here.
             */

            //If there's no last heard sound or the existing one doesn't match, create a new one
            if ((LastHeardSound.Value == null) ||
                (LastHeardSound.Value.SoundType != closestSound.SoundType || LastHeardSound.Value.Location != closestSound.gameObject.transform.position))
            {
                Debug.Log("Updating sound data");

                //Destroy old sound if applicable
                if (LastHeardSound.Value != null)
                {
                    UnityEngine.Object.Destroy(LastHeardSound.Value);
                }

                //Create new sound scriptable object
                LastHeardSound.Value = ScriptableObject.CreateInstance<LastHeardSound>();
                LastHeardSound.Value.Location = closestSound.gameObject.transform.position;
                LastHeardSound.Value.SoundType = closestSound.SoundType;
            }

            return true;

        }

        //Still tracking a sound but don't hear anything new, keep following
        else if (LastHeardSound.Value != null)
        {
            Debug.Log("No new sounds, holding old data");
            return true;
        }

        //List is empty and no sound being tracked
        return false;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
