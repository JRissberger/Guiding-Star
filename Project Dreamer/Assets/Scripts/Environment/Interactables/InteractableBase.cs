using UnityEngine;

public abstract class InteractableBase : MonoBehaviour
{
    //Generic interact method. All responses to interact should be handled in this in child classes.
    public abstract void Interact();
}
