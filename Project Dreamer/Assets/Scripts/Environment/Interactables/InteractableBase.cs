using UnityEngine;

public abstract class InteractableBase : MonoBehaviour
{
    [Tooltip("Duration of cooldown after this item is interacted with")]
    [SerializeField] private float interactCD;

    [Tooltip("Is the interactable one use only?")]
    [SerializeField] private bool oneUse;
    private float timer;

    private bool canInteract;
    public bool CanInteract { get { return canInteract; } }

    //Generic interact method. All child classes should take the interact base + add on their own unique response
    public virtual void Interact()
    {
        //Disables interaction until cooldown expires
            //If it's a one use item, this cooldown won't update, leaving it unusable
        canInteract = false;
        timer = interactCD;
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
