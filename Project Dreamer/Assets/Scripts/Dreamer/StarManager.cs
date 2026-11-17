using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Behavior;
using UnityEngine;

public class StarManager : MonoBehaviour
{
    private StarState starCurrentState;
    private BehaviorGraphAgent behaviorGraph;

    private bool isFearless = false;
    public bool IsFearless { get { return isFearless; } set { isFearless = value; } }

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

    /// <summary>
    /// Checks Star's current state and updates display accordingly
    /// </summary>
    public void UpdateStar(StarState newState)
    {
        starCurrentState = newState;

        //Check Star's current state
        Debug.Log("Star is now " + starCurrentState);

        //Enable/disable corresponding display
    }
}

//States for Star--will likely update if we need more precise ones fo
public enum StarState
{
    Scared,
    Curious,
    Wander
}
