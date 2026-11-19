using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Behavior;
using UnityEngine;

public class StarManager : MonoBehaviour
{
    private StarState starCurrentState;
    private BehaviorGraphAgent behaviorGraph;

    private bool isFearless = false;
    public bool IsFearless { get { return isFearless; } set { isFearless = value; } }

    [Tooltip("The radius around Star where the player is blocked from throwing")]
    [SerializeField] private float playerThrowRadius = 1.0f;
    public float PlayerThrowRadius { get { return playerThrowRadius; } }

    //The height above star where the player isn't allowed to throw. Star height x2
    private float playerThrowHeight;
    public float PlayerThrowHeight {  get { return playerThrowHeight; } }

    private void Start()
    {
        behaviorGraph = GetComponent<BehaviorGraphAgent>();

        if (!behaviorGraph)
        {
            Debug.LogWarning("Behavior graph was not found!");
        }

        //Get the collider to get Star's height
        Collider collider = gameObject.GetComponent<Collider>();
        playerThrowHeight = collider.bounds.size.y * 2;

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

//States for Star--will likely update if we need more precise ones
public enum StarState
{
    Scared,
    Curious,
    Wander
}
