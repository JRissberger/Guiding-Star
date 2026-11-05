using UnityEngine;

public class Memory : MonoBehaviour
{
    [Tooltip("The 2D memory that will be displayed")]
    [SerializeField] private GameObject memoryUI;

    [Tooltip("If there's an interactable (door) this should trigger")]
    [SerializeField] private InteractableBase door;

    [Tooltip("How long the 2D memory should be shown on screen")]
    [SerializeField] private float memoryViewTimer;

    private bool viewingMemory = false;

    //Reference to the star manager to temporarily override the tree
    [Tooltip("Used to pause Star while viewing a memory")]
    [SerializeField] private GameObject star;

    // Update is called once per frame
    void Update()
    {
        if (viewingMemory)
        {
            memoryViewTimer -= Time.deltaTime;

            //When the timer expires, destroy this object
            if (memoryViewTimer <= 0)
            {
                //If there's an interact to run, run it
                if (door)
                {
                    door.Interact();
                }

                //Re enable behavior
                star.GetComponent<StarManager>().ToggleBehavior();

                Destroy(this.gameObject);
            }
        }
    }

    /// <summary>
    /// Marks the memory as collected--will eventually send a signal to patches
    /// </summary>
    private void CollectMemory()
    {
        //TODO: Add connection to memory collection here!

        //Freeze Star
        star.GetComponent<StarManager>().ToggleBehavior();

        //Update the memory in view
        RemoveItemFromView();

        //Enable memory ui
        memoryUI.SetActive(true);

        //Viewing memory--starts display timer
        viewingMemory = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        //Is it Star?
        if (other.CompareTag("Star"))
        {
            CollectMemory();
        }
    }

    /// <summary>
    /// Fully removes the memory from view for Star so it can't get re-targeted
    /// </summary>
    private void RemoveItemFromView()
    {
        //Change this memory's layer so it doesn't get picked up again
            //TODO: Does this need to be on a more specific layer?
        this.gameObject.layer = LayerMask.NameToLayer("Default");

        //Remove the item from the sight view
        star.GetComponent<DreamerSight>().RemoveFromView(this.gameObject);

    }
}
