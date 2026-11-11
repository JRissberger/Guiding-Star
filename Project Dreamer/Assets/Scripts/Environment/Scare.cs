using UnityEngine;

public class Scare : MonoBehaviour
{
    //This object is attached and hidden initially, and will become separated when the scare is destroyed
    [Tooltip("This object will become the primary object when revealed")]
    [SerializeField] private GameObject normalObject;

    private void Start()
    {
        //Hide the normal object for now
        normalObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.LogWarning("Entered!" + other.gameObject.name);
        //Check that the object has the light hazard script and isn't on either of Star's layers
        if (other.gameObject.TryGetComponent<LightHazard>(out LightHazard light) &&
            other.gameObject.layer != LayerMask.NameToLayer("Star") 
            && other.gameObject.layer != LayerMask.NameToLayer("DreamerCompoundSight"))
        {
            RevealObject();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //Check that the object has the light hazard script and isn't on either of Star's layers
        if (other.gameObject.TryGetComponent<LightHazard>(out LightHazard light) &&
            other.gameObject.layer != LayerMask.NameToLayer("Star")
            && other.gameObject.layer != LayerMask.NameToLayer("DreamerCompoundSight"))
        {
            Debug.LogWarning("light");
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

        //Destroy this scare object
        Destroy(gameObject);
    }
}
