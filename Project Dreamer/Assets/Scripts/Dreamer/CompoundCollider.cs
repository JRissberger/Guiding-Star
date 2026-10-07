using UnityEngine;

public class CompoundCollider : MonoBehaviour
{
    private DreamerSight parentSight;
    
    private void Start()
    {
        //Get the parent reference to call during collision
        parentSight = GetComponentInParent<DreamerSight>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //Passes collision data to be handled in the manager
        if (parentSight)
            
        {
            Debug.Log("Item");
            parentSight.TriggerEnter(other);
        }
        else
        {
            Debug.Log("No parent");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Same as enter, handled by the manager
        if (parentSight)
        {
            parentSight.TriggerExit(other);
        }
    }

    
}
