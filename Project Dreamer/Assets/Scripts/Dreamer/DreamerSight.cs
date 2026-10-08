using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;

public class DreamerSight : MonoBehaviour
{
    //List of the current visible objects
    private List<GameObject> visibleItems = new List<GameObject>();

    //All items within sight triggers (visible and non visible)
    private List<GameObject> triggeredItems = new List<GameObject>();

    public List<GameObject> VisibleItems { get { return visibleItems; } }
    

    //When a trigger enter is recognized, check for item tag, then check raycast to see if item is behind wall
    private void OnTriggerEnter(Collider other)
    {
        //Check if it has the StarInteractable tag
        //TODO: Tagging/layers may get updated down the line
        if (other.CompareTag("Interactable"))
        {
            //Adds to triggered list if it's not already there
            if (!triggeredItems.Contains(other.gameObject))
            {
                triggeredItems.Add(other.gameObject);
            }

            //Linecast to check if it's an item in view since the collider can go through walls. ignores collider layer
            //NOTE: Might be good to have a specific layer for obstacles, filter specifically for that. bring up with team?
            //Also checks if the item's already in the list, done since it could collide with multiple triggers at once
            if (CheckIfVisible(other.gameObject)
                && !VisibleItems.Contains(other.gameObject))
            {
                //Adds to visible list
                visibleItems.Add(other.gameObject);
                Debug.Log("Visible items: " + visibleItems.Count);
            }

           
        }

    }

    //Remove the object from the visible and trigger list
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            visibleItems.Remove(other.gameObject);
            triggeredItems.Remove(other.gameObject);
            Debug.Log("Visible items: " + visibleItems.Count);
        }
    }

    private void Update()
    {
        //Run checks on items that have stayed within the collision hitbox to see if they've entered/exited sight
        UpdateVisibility();
    }

    /// <summary>
    /// Linecast to check if it's an item in view since the collider can go through walls. ignores collider layer
    /// NOTE: Might be good to have a specific layer for obstacles, filter specifically for that. bring up with team?
    /// </summary>
    /// <param name="item">The item being checked</param>
    /// <returns></returns>
    private Boolean CheckIfVisible(GameObject item)
    {
        if(Physics.Linecast(transform.position, item.transform.position, out RaycastHit hit, ~LayerMask.GetMask("DreamerCompoundSight")))
        {
            if (hit.transform.gameObject == item)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Checks all items within the triggered range to check if any have entered/exited visible range
    /// </summary>
    private void UpdateVisibility()
    {
        //Loops through all triggered items
        foreach(GameObject item in triggeredItems)
        {
            //Adds it to the list if it's not there but visible
            if (CheckIfVisible(item) && !VisibleItems.Contains(item.gameObject))
            {
                VisibleItems.Add(item);
            }

            //If it's no longer visible, remove from list (doesn't crash if item wasn't there to begin with)
            else
            {
                VisibleItems.Remove(item);
            }
        }
    }
}
