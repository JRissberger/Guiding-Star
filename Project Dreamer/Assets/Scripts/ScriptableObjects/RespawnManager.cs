using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RespawnManager : MonoBehaviour
{
    [SerializeField] private LevelChunk[] _levelChunks;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private MapSegmentManager _mapSegmentManager;
    private GameObject currentChunk;

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

    void Start()
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

        print(_resetMapSegment);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NewRespawn(RespawnTrigger trigger)
    {
        if (trigger)
        {
            foreach (var chunk in _levelChunks)
            {
                if (chunk.RespawnTrigger == trigger)
                {
                    currentChunk = chunk.Chunk;
                }
            }
        }
    }

    public void Respawn()
    {
        _mapSegmentManager = _resetMapSegment;

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
        //_levelChunks.Initialize();

        //reset the relevant area
        // may also handle fadein / fadeout?
    }
}
