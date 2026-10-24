using UnityEngine;

public class ChangeWanderStateTrigger : MonoBehaviour
{

    private enum Actor
    {
        Player,
        Star
    }

    private enum EnableDisable
    {
        Enable, Disable
    }

    [Tooltip("Which actor triggers this?")]
    [SerializeField] private Actor actor;

    [Tooltip("Does this ENABLE or DISABLE CanWander?.")]
    [SerializeField] private EnableDisable enableDisable;

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

        //enables or disabled based on given enum
        if (other.gameObject.name == actorName)
        {
            if (enableDisable == EnableDisable.Enable)
            {
                LevelManager.Instance.EnableStarWander();
            }

            if (enableDisable == EnableDisable.Disable) 
            {

                LevelManager.Instance.DisableStarWander();
            }

            //if oneShot is true, turn off this script.
            if (oneShot)
            {
                this.enabled = false;
            }
        }
    }
}
