using UnityEngine;

public class DoorInteractable : InteractableBase
{
    [Tooltip("The initial door state. Don't disable the prefab children, the correct one will be hidden on runtime!")]
    [SerializeField] private bool isOpen = false;

    private GameObject openDoor;
    private GameObject closeDoor;

    private void Start()
    {
        //Get initial status and open/close children
        openDoor = transform.Find("Open").gameObject;
        closeDoor = transform.Find("Closed").gameObject;

        //Disable the corresponding gameobject
        UpdateDoors();
    }

    public override void Interact()
    {
        //Toggle door state
        isOpen = !isOpen;

        //Update shown door state
        UpdateDoors();

        base.Interact();
    }

    private void UpdateDoors()
    {
        openDoor.SetActive(isOpen);
        closeDoor.SetActive(!isOpen);
    }
}
