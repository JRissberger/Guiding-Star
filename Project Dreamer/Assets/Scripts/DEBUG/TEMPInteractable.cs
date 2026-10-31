using UnityEngine;

//Placeholder deriving from general interactable base, being used for testing
public class TEMPInteractable : InteractableBase
{
    public override void Interact()
    {
        Debug.Log("Interacted with " + this.gameObject.name);
    }
}
