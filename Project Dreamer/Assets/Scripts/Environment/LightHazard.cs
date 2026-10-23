using UnityEngine;

public class LightHazard : MonoBehaviour
{
    [Tooltip("How long it takes for light to kill the player, in seconds.")]
    [SerializeField] private float _lightKillTime = 1.0f;

    private bool _playerInLight = false;
    private float _inLightTimer = 0.0f;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            _playerInLight = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
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
        Debug.Log("Player is dead");
        _playerInLight = false; //temporary, to prevent debug log spam
    }

    /// <summary>
    /// Run every frame if the player is in this light
    /// </summary>
    private void PlayerInLight()
    {
        //Do nothing (for now)
    }
}
