using System;
using UnityEngine;

[Serializable]
public class LevelChunk
{
    [SerializeField] private GameObject chunk;

    public GameObject Chunk {  get { return chunk; } }

    [SerializeField] private Transform respawnChunk;

    
    [SerializeField] private RespawnTrigger respawnTrigger;

    public RespawnTrigger RespawnTrigger {  get { return respawnTrigger; }}


}
