using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A custom collision trigger that fires a UnityEvent.
/// Can be configured to check for the player or for Star.
/// Can also be configured to activate repeatedly as opposed to only once.
/// Useful for scripted events in the level.
/// 
/// TODO: Look into unifying ChangeWanderStateTrigger and ChangeWanderPointsTrigger into uses of this trigger.
/// </summary>
public class CustomEventTrigger : MonoBehaviour
{
    [SerializeField] private UnityEvent OnTrigger;

    private enum Actor
    {
        Player,
        Star
    }

    [Tooltip("Which actor should this trigger check for?")]
    [SerializeField] Actor actor = Actor.Star;

    [Tooltip("Is this a one-shot trigger? (Should it disable itself after activating?)")]
    [SerializeField] private bool oneShot = true;

    private string actorTag = "";

    private void Start()
    {
        switch(actor)
        {
            case Actor.Player:
                actorTag = "Player";
                break;
            case Actor.Star:
                actorTag = "Star";
                break;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(actorTag))
        {
            OnTrigger.Invoke();
            if (oneShot)
            {
                gameObject.GetComponent<Collider>().enabled = false;
            }
        }
    }
}
