using Unity.VisualScripting;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    [SerializeField] private LevelChunk[] _levelChunks;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private GameObject currentChunk;

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


        _levelChunks.Initialize();
        //reset the relevant area
        // may also handle fadein / fadeout?
    }
}
