using UnityEngine;

public class RespawnTrigger : MonoBehaviour
{


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            RespawnManager.Instance.NewRespawn(this);
            this.enabled = false;
        }
    }
}
