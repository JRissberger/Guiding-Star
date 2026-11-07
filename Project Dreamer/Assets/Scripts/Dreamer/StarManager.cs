using Unity.Behavior;
using UnityEngine;

public class StarManager : MonoBehaviour
{
    private BehaviorGraphAgent behaviorGraph;

    private void Start()
    {
        behaviorGraph = GetComponent<BehaviorGraphAgent>();

        if (!behaviorGraph)
        {
            Debug.LogWarning("Behavior graph was not found!");
        }
    }

    /// <summary>
    /// Toggles an override to temporarily fully disable the behavior tree
    /// </summary>
    public void ToggleBehavior()
    {
        behaviorGraph.enabled = !behaviorGraph.enabled;
    }
}
