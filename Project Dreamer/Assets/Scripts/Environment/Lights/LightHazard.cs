using System;
using UnityEngine;

public class LightHazard : MonoBehaviour
{
    [Tooltip("How long it takes for light to kill the player, in seconds.")]
    [SerializeField] private float _lightKillTime = 1.0f;

    private bool _playerInLight = false;
    private float _inLightTimer = 0.0f;

    //Whether the light is currently active or not
    [SerializeField] private bool isActive;
    public bool IsActive { get { return isActive; }  set { isActive = value; } }

    //Spotlight component
    Light spotlight;

    //Action signal sent to scares as needed
    public event Action OnLightOn;

    private void Start()
    {
        //Updates activity based on if the spotlight component is on or not
        Transform spotlightObject = transform.Find("Spot Light");
        if (spotlightObject && spotlightObject.TryGetComponent<Light>(out spotlight))
        {
            //Adjust activity if needed
            spotlight.enabled = isActive;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && isActive)
        {
            _playerInLight = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player" && isActive)
        {
            _playerInLight = false;
            _inLightTimer = 0.0f;
        }
    }

    private void Update()
    {
        if (_playerInLight)
        {
            _inLightTimer += Time.deltaTime;

            PlayerInLight();

            if (_inLightTimer >= _lightKillTime)
            {
                KillPlayer();
            }
        }
    }

    /// <summary>
    /// Run once the player has been in the light too long
    /// </summary>
    private void KillPlayer()
    {
        RespawnManager.Instance.Respawn();
    }

    /// <summary>
    /// Run every frame if the player is in this light
    /// </summary>
    private void PlayerInLight()
    {
        //Do nothing (for now)
    }

    /// <summary>
    /// Toggles the light attached to the prefab
    /// </summary>
    public void ToggleLight()
    {
        spotlight.enabled = !isActive;
        isActive = !isActive;

        if (isActive)
        {
            //TODO: check this--seems to be throwing a warning
            OnLightOn.Invoke();
        }
    }
}
