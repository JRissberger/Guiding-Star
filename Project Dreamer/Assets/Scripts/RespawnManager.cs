using System.Collections;
using System.Collections.Generic;
using Unity.Behavior;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RespawnManager : MonoBehaviour
{
    private struct StarBehaviorData
    {
        public MapSegmentManager mapSegments;
        public SoundManager soundManager;
        public bool canWander;

        public StarBehaviorData(MapSegmentManager mapSegments, SoundManager soundManager, bool canWander)
        {
            this.mapSegments = mapSegments;
            this.soundManager = soundManager;
            this.canWander = canWander;
        }
    }

    private struct ThrowableItemData
    {
        public ThrowableItem instance;
        public Vector3 position;
        public Quaternion rotation;

        public ThrowableItemData(ThrowableItem instance, Vector3 position, Quaternion rotation)
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

    private struct InteractableData
    {
        public InteractableBase instance;

        public InteractableData(InteractableBase instance)
        {
            this.instance = instance;
        }
    }

    [Header("Level Data")]
    [SerializeField] private MapSegmentManager _mapSegmentManager;

    [Header("Instances & Prefabs")]
    [SerializeField] private Image _screenFadeImage;
    [SerializeField] private GameObject _starPrefab;

    private GameObject _playerInstance;
    private GameObject _starInstance;
    private CameraController _cameraController;

    private bool _isRespawning = false;

    private Vector3 _savedPlayerPos;
    private float _savedPlayerAngle;
    private Vector3 _savedStarPos;
    private float _savedStarAngle;
    private int _savedCameraKnot;

    private StarBehaviorData _savedStarBehavior = new();
    private List<ThrowableItemData> _savedThrowables = new();
    private List<MapSegmentData> _savedMapSegments = new();
    private List<LightData> _savedLights = new();
    private List<WanderStateTriggerData> _savedStateTriggers = new();
    private List<WanderPointTriggerData> _savedPointTriggers = new();
    private List<InteractableData> _savedInteractables = new();

    public static RespawnManager Instance { get; private set; }

    private void Awake()
    {
        //Singleton structure. Make sure only one RespawnManager ever exists

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
        _playerInstance = GameObject.FindGameObjectWithTag("Player");
        _starInstance = GameObject.FindGameObjectWithTag("Star");
        _cameraController = FindAnyObjectByType<CameraController>();

        NewRespawn();

        _screenFadeImage.color = new Color(0, 0, 0, 0);

        //On level load, save all the things that will need to be reset to their starting state
        foreach (ThrowableItem item in FindObjectsByType<ThrowableItem>(FindObjectsSortMode.None))
        {
            _savedThrowables.Add(new ThrowableItemData(item, item.transform.position, item.transform.rotation));
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

        foreach (InteractableBase interact in FindObjectsByType<InteractableBase>(FindObjectsSortMode.None))
        {
            _savedInteractables.Add(new InteractableData(interact));
        }
    }

    /// <summary>
    /// Saves the current positions and states of the player and Star.
    /// These will be loaded when Respawn() is called.
    /// </summary>
    /// <param name="player"></param>
    public void NewRespawn()
    {
        _savedPlayerPos = _playerInstance.transform.position;
        _savedPlayerAngle = _playerInstance.transform.rotation.eulerAngles.y;

        _savedStarPos = _starInstance.transform.position;
        _savedStarAngle = _starInstance.transform.rotation.eulerAngles.y;

        _savedCameraKnot = _cameraController.goal;

        BlackboardReference starBehavior = _starInstance.GetComponent<BehaviorGraphAgent>().BlackboardReference;
        BlackboardVariable mapSegmentsVar, soundManagerVar, canWanderVar;
        starBehavior.GetVariable("Map Segments", out mapSegmentsVar);
        starBehavior.GetVariable("SoundManager", out soundManagerVar);
        starBehavior.GetVariable("Can Wander", out canWanderVar);
        MapSegmentManager mapSegments = (MapSegmentManager)mapSegmentsVar.ObjectValue;
        SoundManager soundManager = (SoundManager)soundManagerVar.ObjectValue;
        bool canWander = (bool)canWanderVar.ObjectValue;
        _savedStarBehavior = new StarBehaviorData(mapSegments, soundManager, canWander);
    }

    public void Respawn()
    {
        if (!_isRespawning)
        {
            StartCoroutine(RespawnCoroutine());
        }
    }

    private IEnumerator RespawnCoroutine()
    {
        _isRespawning = true;

        float fade = 0.0f;
        while (fade < 1.0f)
        {
            fade += Time.deltaTime * 2;
            _screenFadeImage.color = new Color(0, 0, 0, fade);
            yield return new WaitForSeconds(Time.deltaTime);
        }

        ResetObjects();
        yield return new WaitForSeconds(0.25f);

        fade = 1.0f;
        while (fade > 0.0f)
        {
            fade -= Time.deltaTime * 2;
            _screenFadeImage.color = new Color(0, 0, 0, fade);
            yield return new WaitForSeconds(Time.deltaTime);
        }

        _isRespawning = false;
    }
    /// <summary>
    /// Loads the states of everything that has been saved previously.
    /// This includes resetting the player and Star,
    /// moving throwables to their starting positions,
    /// resetting lights that were turned off,
    /// and so on.
    /// </summary>
    private void ResetObjects()
    {
        //Reset the player
        _playerInstance.transform.position = _savedPlayerPos;
        _playerInstance.transform.rotation = Quaternion.Euler(0, _savedPlayerAngle, 0);
        _playerInstance.GetComponent<PlayerThrowing>().DropItem();
        _playerInstance.GetComponent<PlayerCorporeality>().SetHiding(false);
        _playerInstance.GetComponent<PlayerCorporeality>().SetCorporeal();
        _playerInstance.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;

        //Reset Star
        Destroy(_starInstance);
        _starInstance = Instantiate(_starPrefab, _savedStarPos, Quaternion.Euler(0, _savedStarAngle, 0));
        BlackboardReference starBehavior = _starInstance.GetComponent<BehaviorGraphAgent>().BlackboardReference;
        starBehavior.SetVariableValue<MapSegmentManager>("Map Segments", _savedStarBehavior.mapSegments);
        starBehavior.SetVariableValue<SoundManager>("SoundManager", _savedStarBehavior.soundManager);
        starBehavior.SetVariableValue<bool>("Can Wander", _savedStarBehavior.canWander);

        //Reset the camera
        _cameraController.MoveTo(_savedCameraKnot, 0);

        //Get a list of every throwable currently in the level.
        //This is used to remove any throwables that didn't exist on level start.
        List<GameObject> throwableObjects = new();
        foreach (ThrowableItem item in FindObjectsByType<ThrowableItem>(FindObjectsSortMode.None))
        {
            throwableObjects.Add(item.gameObject);
        }

        foreach (ThrowableItemData data in _savedThrowables) {
            GameObject instance = data.instance.gameObject;
            instance.transform.position = data.position;
            instance.transform.rotation = data.rotation;

            Rigidbody rb = instance.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            //Remove this throwable (which existed at the start of the level) from the list from earlier.
            throwableObjects.Remove(instance);
        }

        //Remove every remaining throwable (for throwables that were created during the level).
        //NOTE: Other objects might reference these items; this will need to be handled!
        //(Example: Star's vision tracks items within sight; Destroy does not count as exiting sight)
        foreach (GameObject remainingItem in throwableObjects)
        {
            Destroy(remainingItem);
        }

        foreach (MapSegmentData data in _savedMapSegments)
        {
            data.instance.IsActive = data.enabled;
        }

        foreach (LightData data in _savedLights)
        {
            if (data.instance.IsActive != data.enabled)
            {
                data.instance.ToggleLight();
            }
        }

        foreach (WanderStateTriggerData data in _savedStateTriggers)
        {
            data.instance.gameObject.GetComponent<Collider>().enabled = data.enabled;
        }

        foreach (WanderPointTriggerData data in _savedPointTriggers)
        {
            data.instance.gameObject.GetComponent<Collider>().enabled = data.enabled;
        }

        foreach (InteractableData data in _savedInteractables)
        {
            data.instance.ResetInteraction();
        }
    }
}
