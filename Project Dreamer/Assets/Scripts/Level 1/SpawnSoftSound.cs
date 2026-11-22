using UnityEngine;

/// <summary>
/// Event script to spawn a soft sound during Memory 1, Room 2.
/// </summary>
public class SpawnSoftSound : MonoBehaviour
{
    [SerializeField] private Vector3 spawnPosition = Vector3.zero;
    [SerializeField] private float soundRange = 10.0f;
    [SerializeField] private float soundDuration = 1.5f;
    public void Fire() {
        FindAnyObjectByType<SoundManager>().SpawnSound(spawnPosition, SoundType.Soft, soundRange, soundDuration, false);
    }
}
