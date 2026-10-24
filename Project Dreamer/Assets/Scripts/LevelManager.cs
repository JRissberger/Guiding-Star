using Unity.Behavior;
using Unity.VisualScripting;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Tooltip("Attach the Star for the level to this script!")]
    [SerializeField] private BehaviorGraphAgent _starGraph;

    public static LevelManager Instance { get; private set; }

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

    public void DisableStarWander()
    {
        if (_starGraph.GetVariable("Can Wander", out BlackboardVariable<bool> CanWander))
        {
            bool setBool = _starGraph.SetVariableValue("Can Wander", false);
        }
        else
        {
            Debug.Log("Can Wander for Star has failed - please assign the reference in the level manager!");
        }
    }

    public void SetWanderPointsActive(int[] newOpen, int[] newClosed)
    {
        if (_starGraph.GetVariable("Map Segments", out BlackboardVariable<MapSegmentManager> mapSegs))
        {
            MapSegmentManager segmentManager = mapSegs;

            foreach (var openPoint in newOpen)
            {
                segmentManager.MapSegments[(int)openPoint].IsActive = true;
            }
            foreach(var closedPoint in newClosed)
            {
                segmentManager.MapSegments[(int)closedPoint].IsActive = false;
            }
        }
    }

    public void SetWanderPointsAll(int[] onlyOpen)
    {
        if (_starGraph.GetVariable("Map Segments", out BlackboardVariable<MapSegmentManager> mapSegs))
        {
            MapSegmentManager segmentManager = mapSegs;

            for (int i = 0; i < segmentManager.MapSegments.Count; i++)
            {
                segmentManager.MapSegments[i].IsActive = false;
            }

            foreach (var openPoint in onlyOpen)
            {
                segmentManager.MapSegments[(int)openPoint].IsActive = true;
            }
        }
    }
}
