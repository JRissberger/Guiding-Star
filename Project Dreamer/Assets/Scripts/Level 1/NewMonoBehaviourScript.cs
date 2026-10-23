using System.Security.Cryptography;
using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("PLAYER ENTER");
        if (other.gameObject.name == "Player")
        {
            int[] open = {4, 5};
            int[] closed = { 1, 2 };
            LevelManager.Instance.SetWanderPointsActive(open, closed);
        }
    }

}
