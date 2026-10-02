using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using System.Collections.Generic;
using Unity.VisualScripting;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Update Last Heard Sound", story: "Update [TargetSound] and [PrevTargetSound]", category: "Action", id: "fe20db0aa2c468bac919aaa397934f8c")]
public partial class UpdateLastHeardSoundAction : Action
{
    [SerializeReference] public BlackboardVariable<LastHeardSound> TargetSound;
    [SerializeReference] public BlackboardVariable<LastHeardSound> PrevTargetSound;
    [SerializeReference] public BlackboardVariable<SoundManager> SoundManager;
    [SerializeReference] public BlackboardVariable<GameObject> Star;

    protected override Status OnStart()
    {
        List<Sound> soundList = SoundManager.Value.HeardSounds;

        //TODO: need to prio any loud sounds over soft sound

        //Update sound if needed from list
        if (soundList.Count != 0)
        {
            //If the last heard sound was loud, filter out soft sounds
            if (PrevTargetSound.Value && PrevTargetSound.Value.SoundType == SoundType.Loud)
            {
                //Loop backwards due to shifting indices
                for (int i = soundList.Count - 1; i >= 0; i--)
                {
                    if (soundList[i].SoundType == SoundType.Soft)
                    {
                        soundList.RemoveAt(i);
                    }
                }
            }

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


            /* IMPORTANT: Scriptable objects aren't gotten by the garbage collector.
             * They need to be deleted manually otherwise it'll eventually cause a memory leak.
             * Current solution is if there's an existing one that doesn't match the data, destroy the old one
             * If there's a memory issue with the project, it probably is from here.
             */

            //Does closest sound data match the previous target sound? Use that if so
            if (PrevTargetSound.Value && PrevTargetSound.Value.Location == closestSound.gameObject.transform.position
                && PrevTargetSound.Value.SoundType == closestSound.SoundType)
            {
                TargetSound.Value = PrevTargetSound.Value;
            }

            //Otherwise, bump target data to previous target and update target
            else
            {
                
                LastHeardSound soundToDestroy = PrevTargetSound.Value;

                //Do this even if the last target is null, since this is done to track if the target sound has changed
                PrevTargetSound.Value = TargetSound.Value;

                //Create new scriptable object for target sound
                TargetSound.Value = ScriptableObject.CreateInstance<LastHeardSound>();
                TargetSound.Value.Location = closestSound.gameObject.transform.position;
                TargetSound.Value.SoundType = closestSound.SoundType;

                //Destroy the last scriptable object for prev target
                UnityEngine.Object.Destroy(soundToDestroy);
            }
        }

        //No sounds heard, but in the process of tracking an old one
        else if (TargetSound.Value)
        {
            //Make the two sounds match, should continue tracking sound without aborting the branch
            PrevTargetSound.Value = TargetSound.Value;
        }

        return Status.Success;
    }

}

