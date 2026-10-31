using System;
using UnityEngine;

[Serializable]
public class LevelChunk
{
    [SerializeField] private GameObject chunkInstance;

    public GameObject ChunkInstance {  get { return chunkInstance; } }

    [SerializeField] private GameObject chunkPrefab;

    public GameObject ChunkPrefab { get { return chunkPrefab; } }

    [SerializeField] private Transform respawnChunk;

    
    [SerializeField] private RespawnTrigger respawnTrigger;

    public RespawnTrigger RespawnTrigger {  get { return respawnTrigger; }}


}
