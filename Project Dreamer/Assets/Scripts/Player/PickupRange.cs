using UnityEngine;

/// <summary>
/// Temporary script to delegate the player's pickup range to a separate GameObject.
/// This is a quick fix to avoid other triggers registering the player's pickup range as part of player's hitbox.
/// </summary>
public class PickupRange : MonoBehaviour
{
    private PlayerThrowing _parent;
    private void Start()
    {
        _parent = GetComponentInParent<PlayerThrowing>();
    }
    private void OnTriggerEnter(Collider other)
    {
        _parent.AddPickupByCollider(other);
    }
    private void OnTriggerExit(Collider other)
    {
        _parent.RemovePickupByCollider(other);
    }
}
