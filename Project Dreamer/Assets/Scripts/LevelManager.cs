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

    /// <summary>
    /// Turns off the CanWander variable for Star.
    /// </summary>
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

    /// <summary>
    /// Turns on the CanWander variable for Star.
    /// </summary>
    public void EnableStarWander()
    {
        if (_starGraph.GetVariable("Can Wander", out BlackboardVariable<bool> CanWander))
        {
            bool setBool = _starGraph.SetVariableValue("Can Wander", true);
        }
        else
        {
            Debug.Log("Can Wander for Star has failed - please assign the reference in the level manager!");
        }
    }

    /// <summary>
    /// Turns on points in the newOpen array, and turns off points in the newClosed array. Any points not listed are not affected.
    /// </summary>
    /// <param name="newOpen">New open poitns</param>
    /// <param name="newClosed">New closed points</param>
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

    /// <summary>
    /// Turns on all points listed in the parameter; all others are disabled
    /// </summary>
    /// <param name="onlyOpen">The points accessible to Star</param>
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
