using UnityEngine;

public abstract class InteractableBase : MonoBehaviour
{
    [Tooltip("Duration of cooldown after this item is interacted with")]
    [SerializeField] private float interactCD;
    private float timer;

    private bool canInteract;
    public bool CanInteract { get { return canInteract; } }

    //Generic interact method. All responses to interact should be handled in this in child classes.
        //If an object doesn't have to have a cooldown, they don't have to call base.interact in their interact method
    public virtual void Interact()
    {
        //Disables interaction until cooldown expires
        canInteract = false;
        timer = interactCD;
    }

    private void Update()
    {
        //Decreases remaining cooldown timer if item is currently un-interactable
        if (!canInteract)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                canInteract = true;
            }
        }
    }
}
