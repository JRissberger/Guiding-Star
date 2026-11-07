using UnityEngine;

public class DoorInteractable : InteractableBase
{
    [Tooltip("The initial door state. Don't disable the prefab children, the correct one will be hidden on runtime!")]
    [SerializeField] private bool isOpen = false;

    private bool startedOpen = false;
    private bool respawnOpen = false;

    private GameObject openDoor;
    private GameObject closeDoor;

    private void Start()
    {
        //Get initial status and open/close children
        openDoor = transform.Find("Open").gameObject;
        closeDoor = transform.Find("Closed").gameObject;

        //Disable the corresponding gameobject
        UpdateDoors();

        //Track the starting state of this door for the respawn system
        startedOpen = isOpen;
    }

    public override void Interact()
    {
        //Toggle door state
        isOpen = !isOpen;

        //Update shown door state
        UpdateDoors();

        //If this is a one-shot door that got opened, it should stay that way on respawn
        if (isOpen && oneUse)
        {
            respawnOpen = true;
        }

        base.Interact();
    }

    public override void ResetInteraction()
    {
        base.ResetInteraction();

        if (!respawnOpen)
        {
            isOpen = startedOpen;
            UpdateDoors();
        }
    }

    private void UpdateDoors()
    {
        openDoor.SetActive(isOpen);
        closeDoor.SetActive(!isOpen);
    }
}
