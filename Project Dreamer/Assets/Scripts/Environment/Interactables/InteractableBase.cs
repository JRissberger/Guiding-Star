using UnityEngine;

public abstract class InteractableBase : MonoBehaviour
{
    [Tooltip("Duration of cooldown after this item is interacted with")]
    [SerializeField] private float interactCD;

    [Tooltip("Is the interactable one use only?")]
    [SerializeField] protected bool oneUse;
    private float timer;

    private bool canInteract = true;
    public bool CanInteract { get { return canInteract; } }

    //Generic interact method. All child classes should take the interact base + add on their own unique response
    public virtual void Interact()
    {
        //Disables interaction until cooldown expires
            //If it's a one use item, this cooldown won't update, leaving it unusable
        canInteract = false;
        timer = interactCD;
    }

    /// <summary>
    /// Resets the interactable to a usable state, for use by the respawn system.
    /// For the base class this just resets the timer, but child classes can add additional behavior if needed.
    /// </summary>
    public virtual void ResetInteraction()
    {
        timer = 0;
        canInteract = true;
    }

    private void Update()
    {
        //Decreases remaining cooldown timer if item is currently un-interactable and can be re-used
        if (!canInteract && !oneUse)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                canInteract = true;
            }
        }
    }
}
