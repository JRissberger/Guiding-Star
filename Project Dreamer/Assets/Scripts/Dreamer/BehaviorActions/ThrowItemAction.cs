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
    [SerializeReference] public BlackboardVariable<GameObject> Sound;
    [SerializeReference] public BlackboardVariable<GameObject> Star;
    [SerializeReference] public BlackboardVariable<Boolean> HoldingItem;

    protected override Status OnStart()
    {
        //Access throwing script on Star
        DreamerThrowing throwing = Star.Value.gameObject.GetComponent<DreamerThrowing>();

        if (throwing != null)
        {
            //NOTE: max range? move closer if out of range?
            throwing.ThrowItemAtTry(Sound.Value.transform.position, Vector3.up);
            HoldingItem.Value = false;
        }
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

