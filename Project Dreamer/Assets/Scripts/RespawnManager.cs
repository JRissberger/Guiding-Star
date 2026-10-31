using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RespawnManager : MonoBehaviour
{
    private struct PositionedObjectData
    {
        public GameObject instance;
        public Vector3 position;
        public Quaternion rotation;

        public PositionedObjectData(GameObject instance, Vector3 position, Quaternion rotation)
        {
            this.instance = instance;
            this.position = position;
            this.rotation = rotation;
        }
    }

    private struct MapSegmentData
    {
        public MapSegment instance;
        public bool enabled;

        public MapSegmentData(MapSegment instance, bool enabled)
        {
            this.instance = instance;
            this.enabled = enabled;
        }
    }

    private struct LightData
    {
        public LightHazard instance;
        public bool enabled;

        public LightData(LightHazard instance, bool enabled)
        {
            this.instance = instance;
            this.enabled = enabled;
        }
    }

    private struct WanderStateTriggerData
    {
        public ChangeWanderStateTrigger instance;
        public bool enabled;

        public WanderStateTriggerData(ChangeWanderStateTrigger instance, bool enabled)
        {
            this.instance = instance;
            this.enabled = enabled;
        }
    }

    private struct WanderPointTriggerData
    {
        public ChangeWanderPointsTrigger instance;
        public bool enabled;

        public WanderPointTriggerData(ChangeWanderPointsTrigger instance,  bool enabled)
        {
            this.instance = instance;
            this.enabled = enabled;
        }
    }

    [Header("Level Data")]
    [SerializeField] private MapSegmentManager _mapSegmentManager;

    [Header("Instances")]
    [SerializeField] private GameObject _playerInstance;
    [SerializeField] private GameObject _starInstance;

    private Vector3 _savedPlayerPos;
    private float _savedPlayerAngle;

    private List<PositionedObjectData> _savedPositionedObjects = new();
    private List<MapSegmentData> _savedMapSegments = new();
    private List<LightData> _savedLights = new();
    private List<WanderStateTriggerData> _savedStateTriggers = new();
    private List<WanderPointTriggerData> _savedPointTriggers = new();

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

    //On level load, save all the things that will need to be reset to their starting state
    private void Start()
    {
        foreach (ThrowableItem item in FindObjectsByType<ThrowableItem>(FindObjectsSortMode.None))
        {
            SavePositionedObject(item.gameObject);
        }

        foreach (MapSegment segment in _mapSegmentManager.MapSegments)
        {
            _savedMapSegments.Add(new MapSegmentData(segment, segment.IsActive));
        }

        foreach (LightHazard light in FindObjectsByType<LightHazard>(FindObjectsSortMode.None))
        {
            _savedLights.Add(new LightData(light, light.IsActive));
        }

        foreach (ChangeWanderStateTrigger trigger in FindObjectsByType<ChangeWanderStateTrigger>(FindObjectsSortMode.None))
        {
            _savedStateTriggers.Add(new WanderStateTriggerData(trigger, trigger.gameObject.GetComponent<Collider>().enabled));
        }

        foreach (ChangeWanderPointsTrigger trigger in FindObjectsByType<ChangeWanderPointsTrigger>(FindObjectsSortMode.None))
        {
            _savedPointTriggers.Add(new WanderPointTriggerData(trigger, trigger.gameObject.GetComponent<Collider>().enabled));
        }
    }
    
    /// <summary>
    /// Stores the position and rotation of the given object.
    /// The object will be reset to this state when a respawn is triggered,
    /// and its velocity will be set to zero.
    /// </summary>
    /// <param name="obj">The object to save</param>
    private void SavePositionedObject(GameObject obj)
    {
        _savedPositionedObjects.Add(new PositionedObjectData(obj, obj.transform.position, obj.transform.rotation));
    }

    public void NewRespawn(GameObject player)
    {
        _savedPlayerPos = player.transform.position;
        _savedPlayerAngle = player.transform.rotation.eulerAngles.y;
    }

    public void Respawn()
    {

        // may also handle fadein / fadeout?

        foreach (PositionedObjectData data in _savedPositionedObjects) {
            GameObject instance = data.instance;
            instance.transform.position = data.position;
            instance.transform.rotation = data.rotation;

            if (instance.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }

        foreach (MapSegmentData data in _savedMapSegments)
        {
            data.instance.IsActive = data.enabled;
        }

        foreach (LightData data in _savedLights)
        {
            data.instance.IsActive = data.enabled;
        }

        foreach (WanderStateTriggerData data in _savedStateTriggers)
        {
            data.instance.gameObject.GetComponent<Collider>().enabled = data.enabled;
        }

        foreach (WanderPointTriggerData data in _savedPointTriggers)
        {
            data.instance.gameObject.GetComponent<Collider>().enabled = data.enabled;
        }

        Debug.Log(_savedPlayerPos);
        _playerInstance.transform.position = _savedPlayerPos;
        _playerInstance.transform.rotation = Quaternion.Euler(0, _savedPlayerAngle, 0);
        //TODO: Handle resetting the player's state (held item, corporeality)
    }
}
