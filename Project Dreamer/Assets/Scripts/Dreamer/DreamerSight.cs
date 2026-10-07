using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class DreamerSight : MonoBehaviour
{
    //List of the current visible objects
    private List<GameObject> visibleItems = new List<GameObject>();

    public List<GameObject> VisibleItems { get { return visibleItems; } }

    //When a trigger enter is recognized, check for item tag, then check raycast to see if item is behind wall
    private void OnTriggerEnter(Collider other)
    {
        //Check if it has the StarInteractable tag
        //TODO: Tagging/layers may get updated down the line
        if (other.CompareTag("Interactable"))
        {
            //Linecast to check if it's an item in view since the collider can go through walls. ignores collider layer
                //NOTE: Might be good to have a specific layer for obstacles, filter specifically for that. bring up with team?
            //Also checks if the item's already in the list, done since it could collide with multiple triggers at once
            if (Physics.Linecast(transform.position, other.transform.position, out RaycastHit hit, ~LayerMask.GetMask("DreamerCompoundSight")) 
                && !VisibleItems.Contains(other.gameObject))
            {
                //check that linecast hit the object
                if (hit.transform.gameObject == other.gameObject)
                {
                    visibleItems.Add(other.gameObject);
                    Debug.Log(visibleItems.Count);
                }
            }

            //DEBUG
            else if (!Physics.Linecast(transform.position, other.transform.position))
            {
                Debug.Log("Item collided but out of view");
            }
        }
    }

    //Remove the object from the visible list
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Interactable") && VisibleItems.Contains(other.gameObject))
        {
            visibleItems.Remove(other.gameObject);
        }
    }

    
}
