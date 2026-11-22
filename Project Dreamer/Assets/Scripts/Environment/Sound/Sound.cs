using Unity.Behavior;
using Unity.VisualScripting;
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

    private AudioSource audioSource;
    [Tooltip("Audio clip to play if this is a loud sound")]
    [SerializeField] private AudioClip audioClipLoud;
    [Tooltip("Audio clip to play if this is a soft sound")]
    [SerializeField] private AudioClip audioClipSoft;

    [Tooltip("The visual effect that indicates this sound's range")]
    [SerializeField] private GameObject effect;
    [Tooltip("Material for the sound's visual effect (loud sound)")]
    [SerializeField] private Material materialLoud;
    [Tooltip("Material for the sound's visual effect (soft sound)")]
    [SerializeField] private Material materialSoft;

    //Is the sound persistent
    private bool isPersistent = false;
    public bool IsPersistent { get { return isPersistent; } set { isPersistent = value; } }

    //Timer for duration of how long it should be around
    private float timerMax = 0;
    public float TimerMax { get { return timerMax; } set { timerMax = value; } }

    private float timer = 0;
    private float effectTargetRadius;
    private Color baseWaveColor;
    private Color baseFresnelColor;
    private MeshRenderer meshRenderer;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        PlayAudio();

        meshRenderer = effect.GetComponent<MeshRenderer>();
        if (soundType == SoundType.Loud)
        {
            meshRenderer.material = materialLoud;
        }
        else if (soundType == SoundType.Soft)
        {
            meshRenderer.material = materialSoft;
        }

        baseWaveColor = meshRenderer.material.GetColor("_WaveColor");
        baseFresnelColor = meshRenderer.material.GetColor("_FresnelColor");
    }

    void Update()
    {
        UpdateTimer();

        float radius = effectTargetRadius * (1.0f - Mathf.Pow(1.0f - Mathf.Sqrt(timer / timerMax), 2));
        float diameter = radius * 2.0f;
        effect.transform.localScale = new Vector3(diameter, diameter, diameter);

        //Fade out in the last 1/4 of the effect time-wise (max 0.5 seconds of fading)
        float alpha;
        if (timerMax >= 2.0f)
        {
            alpha = (timerMax - timer) * 2.0f;
        }
        else
        {
            alpha = (timerMax - timer) / timerMax * 4.0f;
        }
        alpha = Mathf.Min(1.0f, alpha);

        Color invisible = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        Color waveColor = meshRenderer.material.GetColor("_WaveColor");
        waveColor = Color.Lerp(invisible, baseWaveColor, alpha);
        meshRenderer.material.SetColor("_WaveColor", waveColor);
        Color fresnelColor = meshRenderer.material.GetColor("_FresnelColor");
        fresnelColor = Color.Lerp(invisible, baseFresnelColor, alpha);
        meshRenderer.material.SetColor("_FresnelColor", fresnelColor);
    }

    //Adjusts the audible range of the sound (modifying trigger radius)
    public void UpdateAudibleRange(float range)
    {
        //Get spherecollider, update radius
        SphereCollider collider = this.GetComponent<SphereCollider>();
        collider.radius = range;

        effectTargetRadius = range;
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

    //Update timer, destroy object when timer runs out
    private void UpdateTimer()
    {
        timer += Time.deltaTime;

        //Destroy self if timer is up
        if (!isPersistent && timer > timerMax)
        {
            soundManager.HeardSounds.Remove(this);
            Destroy(this.gameObject);
        }
    }

    //Play the audio for this sound (loud or soft)
    private void PlayAudio()
    {
        if (soundType == SoundType.Loud)
        {
            audioSource.PlayOneShot(audioClipLoud);
        }
        else if (soundType == SoundType.Soft)
        {
            audioSource.PlayOneShot(audioClipSoft);
        }
    }
}
