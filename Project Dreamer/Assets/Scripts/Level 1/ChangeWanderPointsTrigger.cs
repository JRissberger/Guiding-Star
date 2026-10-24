using UnityEngine;

public class ChangeWanderPointsTrigger : MonoBehaviour
{
    private enum Actor
    {
        Player,
        Star
    }

    private enum ChangeMethod
    {
        SetActive,
        SetAll
    }

    [SerializeField] private Actor actor;

    [SerializeField] private ChangeMethod changeMethod;

    [Tooltip("USED IN SETACIVE ONLY: List of ints for all WanderPoints that should be enabled.")]
    [SerializeField] private int[] activeOpenPoints;

    [Tooltip("USED IN SETACTIVE ONLY: List of ints for all WanderPoints that should be disabled.")]
    [SerializeField] private int[] activeClosedPoints;

    [Tooltip("USED IN SETALL ONLY: List of ints for all WanderPoints that should be enabled. All other points are disabled.")]
    [SerializeField] private int[] allOpenPoints;

    [Tooltip("Can this script only be used once?")]
    [SerializeField] private bool oneShot;
    public void OnTriggerEnter(Collider other)
    {
        string actorName = null;
        if (actor == Actor.Player)
        {
            actorName = "Player";
        }
        if (actor == Actor.Star)
        {
            actorName = "Star";
        }

        if (other.gameObject.name == actorName)
        {
            if (changeMethod == ChangeMethod.SetActive)
            {
                LevelManager.Instance.SetWanderPointsActive(activeOpenPoints, activeClosedPoints);
            }

            if (changeMethod == ChangeMethod.SetAll)
            {
                LevelManager.Instance.SetWanderPointsAll(allOpenPoints);
            }
            //if oneShot is true, disable this script.
            if (oneShot)
            {
                this.enabled = false;
            }
        }

    }
}
