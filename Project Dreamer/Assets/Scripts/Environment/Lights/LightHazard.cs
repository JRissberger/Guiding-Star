using System;
using UnityEngine;

public class LightHazard : MonoBehaviour
{
    [Tooltip("How long it takes for light to kill the player, in seconds.")]
    [SerializeField] private float _lightKillTime = 1.0f;

    //Temporary field for the fog cone effect, to turn it on/off alongside the light itself
    [SerializeField] private GameObject lightFog = null;
    PlayerCorporeality player;

    private bool _playerInLight = false;
    private float _inLightTimer = 0.0f;
    //Remember the player instance that touched this light
    //Avoids issues with the player being destroyed by the respawn system
    //(since destruction doesn't count as exiting the light)
    private GameObject _storedPlayerInstance = null;

    //Whether the light is currently active or not
    [SerializeField] private bool isActive;
    public bool IsActive { get { return isActive; }  set { isActive = value; } }

    //Spotlight component
    Light spotlight;

    //Action signal sent to scares as needed
    public event Action OnLightOn;

    [Tooltip("Check this if it's a light attached to Star's vision cone!")]
    [SerializeField] private bool IsOnStar = false;

    //References player
    private GameObject player;
    private PlayerCorporeality playerCorporeality;

    private void Start()
    {
        //Updates activity based on if the spotlight component is on or not
        Transform spotlightObject = transform.Find("Spot Light");
        if (spotlightObject && spotlightObject.TryGetComponent<Light>(out spotlight))
        {
            //Adjust activity if needed
            spotlight.enabled = isActive;
        }
        if (lightFog)
        {
            lightFog.SetActive(isActive);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && isActive)
        {
            //Reference to player
            player = other.gameObject;
            playerCorporeality = other.gameObject.GetComponent<PlayerCorporeality>();
            _playerInLight = true;
            _storedPlayerInstance = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player" && isActive)
        {
            _playerInLight = false;
            _inLightTimer = 0.0f;
            _storedPlayerInstance = null;
        }
    }

    private void Update()
    {
        if (_storedPlayerInstance == null)
        {
            _playerInLight = false;
            _inLightTimer = 0.0f;
        }
        else if (_playerInLight && !player.IsHiding())
        //Extra checks are run if this is a light on Star, to account for their light cone mesh potentially clipping through a wall
        if (_playerInLight && IsOnStar && !playerCorporeality.IsHiding())
        {
            //Make sure there's a direct line between star and the player (avoiding clipping through walls incidents)
                //Using transform.root to use Star's position, since the cone's transform is in the middle
            if (Physics.Linecast(transform.root.position, player.transform.position, out RaycastHit hit, ~LayerMask.GetMask("DreamerCompoundSight", "Star")))
            { 
                if (hit.transform.gameObject == player)
                {
                    _inLightTimer += Time.deltaTime;

                    PlayerInLight();

                    if (_inLightTimer >= _lightKillTime)
                    {
                        KillPlayer();
                    }
                }
            }

            else
            {
                Debug.LogWarning("Player in cone but linecast is blocked!");
            }

        }

        //Standard light, calculate normally without linecast
        else if (_playerInLight && !playerCorporeality.IsHiding())
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
        if (lightFog)
        {
            lightFog.SetActive(!isActive);
        }
        isActive = !isActive;

        if (isActive && OnLightOn != null)
        {
            //TODO: check this--seems to be throwing a warning
            OnLightOn.Invoke();
        }
    }
}
