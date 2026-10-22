using UnityEngine;
using UnityEngine.Events;

public class ThrowableItem : MonoBehaviour
{
    [Header("Sound Settings")]

    [Tooltip("The type of sound this object creates when landing. \"None\" will create no sound at all.")]
    [SerializeField] private SoundType _soundType = SoundType.None;

    [Tooltip("How far away this object's landing sound is audible from.")]
    [SerializeField] private float _soundRadius = 1.0f;

    [Tooltip("How long the sound this object makes will last, in seconds.")]
    [SerializeField] private float _soundDuration = 1.0f;

    [Header("Other")]

    [Tooltip("Whether this item disappears after being thrown.")]
    [SerializeField] private bool _willDespawn = false;

    //Called when an object is finished being thrown and lands on a surface.
    //Only fires on the first impact after being thrown.
    //As arguments, passes the thrown object, and whether this throw created a sound (true) or not (false).
    [System.NonSerialized] public UnityEvent<GameObject, bool> _throwLandEvent = new UnityEvent<GameObject, bool>();

    private bool _isMidThrow = false;
    private bool _makeLandingSound = false;
    private SoundManager _soundManager;

    private void Start()
    {
        _soundManager = FindAnyObjectByType<SoundManager>();
    }

    /// <summary>
    /// A method to be called when this item is thrown.
    /// The next time the item collides with a surface, it will run landing-related code.
    /// To get the object to make a sound, use QueueLandSound() in combination with this.
    /// </summary>
    public void SetThrown()
    {
        _isMidThrow = true;
    }

    /// <summary>
    /// When called, the next time this item lands, it will create its associated landing sound (determined in the inspector).
    /// Make sure to also call SetThrown() to run the normal landing code! The landing sound is also dependent on that being run.
    /// </summary>
    public void QueueLandSound()
    {
        _makeLandingSound = true;
    }

    /// <summary>
    /// A method to be called when this item lands, regardless of the thrower.
    /// This will destroy this item if "Will Despawn" is checked.
    /// If the player threw this item, it will also create a SoundManager sound, with parameters based on the values set in the inspector.
    /// </summary>
    private void Land()
    {
        if (_makeLandingSound && _soundType != SoundType.None)
        {
            _soundManager.SpawnSound(transform.position, _soundType, _soundRadius, _soundDuration, false);
        }

        _throwLandEvent.Invoke(gameObject, _makeLandingSound);

        if (_willDespawn)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Gets whether the item has just been thrown.
    /// This variable is set to true when an object is thrown, and false when it collides with a surface.
    /// </summary>
    /// <returns>True if the item is mid-throw, false otherwise</returns>
    public bool IsMidThrow()
    {
        return _isMidThrow;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_isMidThrow)
        {
            Land();
            _isMidThrow = false;
        }
    }
}
