using UnityEngine;

public class CORELOOPOepnVent : MonoBehaviour
{
    [SerializeField] private GameObject vent;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            vent.SetActive(false);
        }
    }
}
