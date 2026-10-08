using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ItemResolved", story: "[Item] investigation resolved", category: "Action", id: "1e91bcff8d509589ec72796fc6c6ae80")]
public partial class ItemResolvedAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Item;
    [SerializeReference] public BlackboardVariable<Boolean> SawItem;

    protected override Status OnStart()
    {
        //Resets values
        Item.Value = null;
        SawItem.Value = false;
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

