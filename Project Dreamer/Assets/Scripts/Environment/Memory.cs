using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Memory : MonoBehaviour
{
    [Tooltip("The 2D memory that will be displayed")]
    [SerializeField] private GameObject memoryUI;

    [Tooltip("If there's an interactable (door) this should trigger")]
    [SerializeField] private InteractableBase door;

    [Tooltip("How long the 2D memory should be shown on screen")]
    [SerializeField] private float memoryViewTimer;

    //Reference to the star manager to temporarily override the tree
    [Tooltip("Used to pause Star while viewing a memory")]
    [SerializeField] private GameObject star;

    private void Start()
    {
        //If there's an interactable, change its layer so Star won't access it
            //NOTE: Update if a more specific layer is needed
        if (door)
        {
            door.gameObject.layer = LayerMask.NameToLayer("Default");
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

        //Start fade in coroutine
            //Note: Will need to add onto this if we ever want multiple images
        StartCoroutine(MemoryView(GetComponentInChildren<Image>()));
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

    /// <summary>
    /// Fades an image into view
    /// </summary>
    /// <param name="image">The image to display</param>
    /// <returns></returns>
    IEnumerator FadeIMG(Image image, bool fadeout = false, float duration = 0.5f)
    {
        float currentTime = 0;

        //Starting point for fade in
        Color alpha = image.color;

        //Fade in over 2 sec
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;

            //Update alpha depending on fade direction, use clamp to keep it between 0 and 1
            if (fadeout)
            {
                alpha.a = Mathf.Clamp01(1f - (currentTime / duration));
            }
            else
            {
                alpha.a = Mathf.Clamp01(currentTime / duration);
            }

            image.color = alpha;
            yield return null;
        }
    }

    /// <summary>
    /// Full timed setup of memory fade in/out and waiting in between
    /// </summary>
    /// <param name="image"></param>
    /// <returns></returns>
    IEnumerator MemoryView(Image image)
    {
        //Fade in memory
        yield return StartCoroutine(FadeIMG(image));

        //Wait until there's 0.5 seconds left on the timer
        yield return new WaitForSeconds(memoryViewTimer - 0.5f);

        //Fade out memory
        yield return StartCoroutine(FadeIMG(image, true));

        //Interact with linked interactable if needed
        if (door)
        {
            door.Interact();

        }

        //Re enable star's behavior tree
        star.GetComponent<StarManager>().ToggleBehavior();

        //Destroy memory
        Destroy(this.gameObject);
    }
}
