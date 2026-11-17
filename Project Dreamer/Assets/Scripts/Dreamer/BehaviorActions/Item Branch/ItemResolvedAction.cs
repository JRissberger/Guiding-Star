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

    private StarManager starManager;

    protected override Status OnStart()
    {
        //Resets values
        Item.Value = null;
        SawItem.Value = false;

        //Get the Star manager and update state to default
        starManager = GameObject.GetComponent<StarManager>();
        starManager.UpdateStar(StarState.Wander);

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

