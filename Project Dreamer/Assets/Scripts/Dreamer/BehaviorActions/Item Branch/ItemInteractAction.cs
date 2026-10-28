using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Item Interact", story: "[Star] interacts with [Item]", category: "Action", id: "86647ccb64837fc09b42fd9d6ded30dd")]
public partial class ItemInteractAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Star;
    [SerializeReference] public BlackboardVariable<GameObject> Item;

    protected override Status OnStart()
    {
        //Make sure the item is interactable and get the script
            //This returns true for scripts that are children of the generic script
        if(Item.Value.TryGetComponent<InteractableBase>(out InteractableBase interactable))
        {
            //Call the interact method. This will access the interact method on the child script that's actually on the object
            interactable.Interact();
        }
        else
        {
            Debug.LogWarning("No interactable script found on " + Item.Value);
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

