using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RespawnManager : MonoBehaviour
{
    [Header("Level Data")]
    [SerializeField] private LevelChunk[] _levelChunks;
    [SerializeField] private MapSegmentManager _mapSegmentManager;

    [Header("Instances")]
    [SerializeField] private GameObject _playerInstance;
    [SerializeField] private GameObject _starInstance;

    private GameObject _currentChunk;
    private Vector3 _playerSpawnPos;
    private float _playerSpawnAngle;

    private MapSegmentManager _resetMapSegment;

    public static RespawnManager Instance { get; private set; }

    private void Awake()
    {
        // If this instance is not the correct instance, bye bye!.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        _resetMapSegment = Instantiate(_mapSegmentManager);

        /*
        //THIS IS A PATCHWORK SOLUTION FOR NOW
        foreach(var mapSegments in _mapSegmentManager.MapSegments)
        {
            mapSegments.IsActive = false;
        }
        _mapSegmentManager.MapSegments[0].IsActive = true;
        */
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NewRespawn(RespawnTrigger trigger, GameObject player)
    {
        if (trigger)
        {
            foreach (var chunk in _levelChunks)
            {
                if (chunk.RespawnTrigger == trigger)
                {
                    _currentChunk = chunk.Chunk;
                }
            }

            _playerSpawnPos = player.transform.position;
            _playerSpawnAngle = player.transform.rotation.eulerAngles.y;
        }
    }

    public void Respawn()
    {
        _mapSegmentManager = _resetMapSegment;

        GameObject newChunk = Instantiate(_currentChunk, _currentChunk.transform.parent);
        Destroy(_currentChunk);
        _currentChunk = newChunk;
        // may also handle fadein / fadeout?

        _playerInstance.transform.position = _playerSpawnPos;
        _playerInstance.transform.rotation = Quaternion.Euler(0, _playerSpawnAngle, 0);
        //TODO: Handle resetting the player's state (held item, corporeality)
    }
}
