using UnityEngine;

/// <summary>
/// Quick code to spawn a throwable item mid-playthrough, currently used to test respawning.
/// </summary>
public class DebugSpawnThrowable : MonoBehaviour
{
    [SerializeField] GameObject throwablePrefab;
    public void SpawnThrowable()
    {
        GameObject player = FindAnyObjectByType<PlayerMovement>().gameObject;

        Instantiate(throwablePrefab, player.transform.position + Vector3.left, Quaternion.identity);
    }
}
