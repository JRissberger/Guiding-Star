using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Unity.VisualScripting;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Throw item", story: "Throw [HeldItem] towards [Sound] location.", category: "Action", id: "c0ce95087eb4b9d7039a90ca62f1e1c4")]
public partial class ThrowItemAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> HeldItem;
    [SerializeReference] public BlackboardVariable<LastHeardSound> Sound;
    [SerializeReference] public BlackboardVariable<GameObject> Star;
    [SerializeReference] public BlackboardVariable<Boolean> HoldingItem;
    [SerializeReference] public BlackboardVariable<float> ThrowRange;

    protected override Status OnStart()
    {
        //Access throwing script on Star
        DreamerThrowing throwing = Star.Value.gameObject.GetComponent<DreamerThrowing>();

        if (throwing != null)
        {

            //Attempts to throw item at location first
            if(throwing.ThrowItemAtTry(Sound.Value.Location, Vector3.up, ThrowRange.Value))
            {
                HoldingItem.Value = false;
                HeldItem.Value = null;
                Debug.Log("Threw to target!");
            }

            //Otherwise, determine a location in range and throw towards that location
            else
            {
                //Normalize vector to get direction to sound
                Vector3 direction = (Sound.Value.Location - Star.Value.transform.position).normalized;

                //Find position on the vector within range
                Vector3 throwTarget = Star.Value.transform.position + direction * ThrowRange.Value;

                //NOTE: may want handling for if there's an obstacle in the way?? but this probably would go under the throwing code
                //If this throw succeeded, update
                if(throwing.ThrowItemAtTry(throwTarget, Vector3.up, ThrowRange.Value))
                {
                    HoldingItem.Value = false;
                    HeldItem.Value = null;
                    Debug.Log("Threw to closest reachable position towards target!");
                }
                //DEBUG
                else
                {
                    Debug.Log("Unable to throw item");
                }

            }
        }

        return Status.Success;
    }
}

