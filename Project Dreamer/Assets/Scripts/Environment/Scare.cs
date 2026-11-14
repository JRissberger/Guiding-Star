using UnityEngine;
using System.Collections.Generic;

public class Scare : MonoBehaviour
{
    //This object is attached and hidden initially, and will become separated when the scare is destroyed
    [Tooltip("This object will become the primary object when revealed")]
    [SerializeField] private GameObject normalObject;

    //All lights currently overlapping with this scare
    private List<LightHazard> lights;

    private void Start()
    {
        lights = new List<LightHazard>();

        //Hide the normal object for now
        normalObject.SetActive(false);

        //Get all the lights currently colliding with this object
        Collider collider = GetComponent<Collider>();
        Collider[] objects = Physics.OverlapBox(
            collider.bounds.center,
            collider.bounds.extents,
            collider.transform.rotation,
            ~0, //Every layer
            QueryTriggerInteraction.Collide);

        //Loop through and find lights
        foreach (Collider other in objects)
        {
            if (other.gameObject.TryGetComponent<LightHazard>(out LightHazard light))
            {
                //Add to list and subscribe to action
                lights.Add(light);
                light.OnLightOn += RevealObject;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Check that the object has the light hazard script and isn't on either of Star's layers
        if (other.gameObject.TryGetComponent<LightHazard>(out LightHazard light) &&
            other.gameObject.layer != LayerMask.NameToLayer("Star") 
            && other.gameObject.layer != LayerMask.NameToLayer("DreamerCompoundSight"))
        {
            //If it's on, run the reveal
            if (light.IsActive)
            {
                RevealObject();
            }

            //Otherwise, add it to the list if it isn't there, and subscribe
            else
            {
                if (!lights.Contains(light))
                {
                    lights.Add(light);
                    light.OnLightOn += RevealObject;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Remove it from the list and unsubscribe
        if (other.gameObject.TryGetComponent<LightHazard>(out LightHazard light) &&
            other.gameObject.layer != LayerMask.NameToLayer("Star")
            && other.gameObject.layer != LayerMask.NameToLayer("DreamerCompoundSight"))
        {
            lights.Remove(light);
            light.OnLightOn -= RevealObject;
        }
    }

    /// <summary>
    /// Removes a scare and reveals the normal object
    /// </summary>
    private void RevealObject()
    {
        //Detach the hidden object from the scare and enable it
        normalObject.SetActive(true);
        normalObject.transform.SetParent(null);

        //Remove off of actions--seems to be causing issues otherwise
        foreach (LightHazard light in lights)
        {
            light.OnLightOn -= RevealObject;
        }

        //Destroy this scare object
        Destroy(gameObject);
    }
}
