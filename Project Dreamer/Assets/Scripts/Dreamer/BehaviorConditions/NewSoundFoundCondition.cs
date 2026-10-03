using System;
using System.Collections.Generic;
using Unity.Behavior;
using Unity.VisualScripting;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "NewSoundFound", story: "Does [TargetSound] not match [NewTargetSound]? [SoundManager] [Star]", category: "Conditions", id: "65f36720bff09d77eef6da9b5f770107")]
public partial class NewSoundFoundCondition : Condition
{
    [SerializeReference] public BlackboardVariable<LastHeardSound> TargetSound;
    [SerializeReference] public BlackboardVariable<LastHeardSound> NewTargetSound;
    [SerializeReference] public BlackboardVariable<SoundManager> SoundManager;
    [SerializeReference] public BlackboardVariable<GameObject> Star;

    public override bool IsTrue()
    {

        List<Sound> soundList = SoundManager.Value.HeardSounds;

        if (soundList.Count > 0)
        {
            //Determine new sound to target
            
            /* Filter out soft sounds for certain cases
             * If the current sound is loud, a soft sound will never overwrite
             * If the current sound is soft and there's any loud sounds in the list
             */

            if (TargetSound.Value && ((TargetSound.Value.SoundType == SoundType.Loud) || 
                TargetSound.Value.SoundType == SoundType.Soft && soundList.Exists(sound => sound.SoundType == SoundType.Loud)))
            {
                //Loop backwards due to shifting indices
                for (int i = soundList.Count - 1; i >= 0; i--)
                {
                    if (soundList[i].SoundType == SoundType.Soft)
                    {
                        soundList.RemoveAt(i);
                    }
                }

                ////DEBUG: check which condition triggered filter
                //if (TargetSound.Value.SoundType == SoundType.Loud)
                //{
                //    Debug.Log("Current sound is loud, filtered");
                //}
                //if (TargetSound.Value.SoundType == SoundType.Soft && soundList.Exists(sound => sound.SoundType == SoundType.Loud))
                //{
                //    Debug.Log("Current sound is soft and there's a loud sound, filtered");
                //}
            }



            //Make sure there's still any valid targets
            if (soundList.Count > 0)
            {
                //Get the closest sound out of the valid ones
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

                //Is it a different value than the current target?
                if ((TargetSound.Value && (TargetSound.Value.SoundType != closestSound.SoundType
                    || TargetSound.Value.Location != closestSound.gameObject.transform.position)) || !TargetSound.Value)
                {
                    /* IMPORTANT: Scriptable objects aren't gotten by the garbage collector.
                    * They need to be deleted manually otherwise it'll eventually cause a memory leak.
                    * Current solution is if there's an existing one that doesn't match the data, destroy the old one
                    * If there's a memory issue with the project, it probably is from here.
                    */

                    //Destroy previous target object if applicable
                    if (TargetSound.Value)
                    {
                        UnityEngine.Object.Destroy(TargetSound.Value);
                    }


                    //Create object for new target
                    NewTargetSound.Value = ScriptableObject.CreateInstance<LastHeardSound>();
                    NewTargetSound.Value.Location = closestSound.gameObject.transform.position;
                    NewTargetSound.Value.SoundType = closestSound.SoundType;

                    return true;
                }
            }
        }

        //No better targets, this branch fails and sequence goes to navigation
        return false;
    }

}
