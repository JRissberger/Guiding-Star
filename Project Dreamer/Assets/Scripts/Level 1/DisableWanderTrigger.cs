using UnityEngine;

public class DisableWanderTrigger : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Star")
        {
            LevelManager.Instance.DisableStarWander();
        }
    }
}
