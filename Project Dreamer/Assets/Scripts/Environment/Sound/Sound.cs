using Unity.Behavior;
using UnityEngine;

public enum SoundType
{
    Soft,
    Loud,
    None //Fallback
}

public class Sound : MonoBehaviour
{
    [SerializeField] private SoundType soundType = SoundType.None;
    public SoundType SoundType { get { return soundType; } set { soundType = value; } }

    private SoundManager soundManager;
    public SoundManager SoundManager { set { soundManager = value; } }

    //Is the sound persistent
    private bool isPersistent = false;
    public bool IsPersistent { get { return isPersistent; } set { isPersistent = value; } }

    //Timer for duration of how long it should be around
    private float timer = 0;
    public float Timer { get { return timer; } set { timer = value; } }

    void Update()
    {
        //Call timer update if not persistent
        if (!isPersistent)
        {
            UpdateTimer();
        }
    }

    /// <summary>
    /// Adjusts the audible range of the sound (modifying trigger radius)
    /// </summary>
    /// <param name="range">Radius of the hearing range collider</param>
    public void UpdateAudibleRange(float range)
    {
        //Get spherecollider, update radius
        SphereCollider collider = this.GetComponent<SphereCollider>();
        collider.radius = range;
    }


    //Adds sound to heard list if star enters range
    private void OnTriggerEnter(Collider other)
    {

        //Check that it's star that entered
        if (other.CompareTag("Star"))
        {
            //Add this sound to the manager's audible sounds list
            soundManager.HeardSounds.Add(this);
        }
    }

    //Removes this sound from audible list when star leaves range
    private void OnTriggerExit(Collider other)
    {
        //Check that it's Star who left
        if (other.CompareTag("Star"))
        {
            //Remove from list
            soundManager.HeardSounds.Remove(this);
        }
    }

    /// <summary>
    /// Update the sound's timer, if applicable. Destroy the sound when timer expires.
    /// </summary>
    private void UpdateTimer()
    {
        timer -= Time.deltaTime;

        //Destroy self if timer is at 0
        if (timer <= 0)
        {
            soundManager.HeardSounds.Remove(this);
            Destroy(this.gameObject);
        }
    }

    //Are we having the actual sound object play a noise?
    //If so, method here for data surrounding playing said noise
}
