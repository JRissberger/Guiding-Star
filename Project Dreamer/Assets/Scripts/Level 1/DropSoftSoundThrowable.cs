using UnityEngine;

/// <summary>
/// Event script to drop a soft throwable when Star enters Memory 2, Room 1.
/// </summary>
public class DropSoftSoundThrowable : MonoBehaviour
{
    [SerializeField] Vector3 spawnPosition = Vector3.zero;
    [SerializeField] GameObject throwablePrefab = null;

    public void Fire()
    {
        ThrowableItem throwable = Instantiate(throwablePrefab, spawnPosition, Quaternion.identity).GetComponent<ThrowableItem>();
        throwable.SetThrown();
        throwable.QueueLandSound();
    }
}
