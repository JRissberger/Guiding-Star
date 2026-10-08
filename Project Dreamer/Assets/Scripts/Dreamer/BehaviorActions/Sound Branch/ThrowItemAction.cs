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
    DreamerThrowing throwing = null;
    Vector3 turnDirection = Vector3.zero;

    protected override Status OnStart()
    {
        //Access throwing script on Star
        throwing = Star.Value.gameObject.GetComponent<DreamerThrowing>();
        turnDirection = Sound.Value.Location - Star.Value.transform.position;
        turnDirection.y = 0;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        //Rotate if needed 
            //NOTE: may need some wiggle room
        if (turnDirection.sqrMagnitude > 0.001f)
        {
            
            //Calc rotation
            Quaternion rotation = Quaternion.LookRotation(turnDirection);

            //Lerp to face target
            Star.Value.transform.rotation = Quaternion.RotateTowards(Star.Value.transform.rotation, rotation, 180 * Time.deltaTime);

            //If rotation isn't done, return running status
            if (Quaternion.Angle(Star.Value.transform.rotation, rotation) > 1)
            {
                Debug.Log("Turning");
                return Status.Running;
            }
        }

        if (throwing != null)
        {

            //Attempts to throw item at location first
            if (throwing.ThrowItemAtTry(Sound.Value.Location, Vector3.up, ThrowRange.Value))
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
                if (throwing.ThrowItemAtTry(throwTarget, Vector3.up, ThrowRange.Value))
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

