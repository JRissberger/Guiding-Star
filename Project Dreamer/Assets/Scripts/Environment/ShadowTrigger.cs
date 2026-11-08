using UnityEngine;

public class ShadowTrigger : MonoBehaviour
{
    PlayerCorporeality corporeality;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        corporeality = GameObject.Find("Player").GetComponent<PlayerCorporeality>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerCorporeality>())
        {
            other.GetComponent<PlayerCorporeality>().OnShadowEnter(this);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerCorporeality>())
        {
            other.GetComponent<PlayerCorporeality>().OnShadowExit(this);
        }
    }
}
