using UnityEngine;

public class Floorboard : MonoBehaviour
{
    [Tooltip("The range of this floorboard's sound, in Unity units")]
    [SerializeField] private float _soundRange = 3.0f;

    [Tooltip("How long this floorboard's sound lasts, in seconds")]
    [SerializeField] private float _soundDuration = 1.0f;

    private SoundManager _soundManager;

    private void Start()
    {
        _soundManager = FindAnyObjectByType<SoundManager>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            _soundManager.SpawnSound(transform.position, SoundType.Loud, _soundRange, _soundDuration, false);
        }
    }
}
