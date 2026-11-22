using UnityEngine;

public class RespawnTrigger : MonoBehaviour
{

    [Tooltip("Transform indicating where Star should spawn if the player dies after hitting this trigger. Also determines Star's facing direction if this happens (Star will face in the +Z direction of the transform).")]
    [SerializeField] Transform starRespawnPoint;

    [Tooltip("What kind of item Star should spawn with, if any")]
    [SerializeField] RespawnManager.RespawnItemType starItem = RespawnManager.RespawnItemType.None;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            RespawnManager.Instance.NewRespawn(starRespawnPoint.position, starRespawnPoint.eulerAngles.y, starItem);
            gameObject.GetComponent<Collider>().enabled = false;
        }
    }

    //Draw Star's spawn point and angle in the editor
    private void OnDrawGizmos() { 
        if (starRespawnPoint != null)
        {
            Gizmos.color = new Color(1.0f, 1.0f, 0.5f, 1.0f);
            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.DrawWireSphere(starRespawnPoint.position, 0.25f);
            Vector3 arrowMid = starRespawnPoint.position + starRespawnPoint.forward * 0.75f;
            Vector3 arrowEnd = starRespawnPoint.position + starRespawnPoint.forward;
            Gizmos.DrawLine(starRespawnPoint.position, arrowEnd);
            //Gizmos.DrawLine(arrowMid + starRespawn.up * 0.25f, arrowEnd);
            Gizmos.DrawLine(arrowMid + starRespawnPoint.right * 0.25f, arrowEnd);
            //Gizmos.DrawLine(arrowMid - starRespawn.up * 0.25f, arrowEnd);
            Gizmos.DrawLine(arrowMid - starRespawnPoint.right * 0.25f, arrowEnd);
        }
    }
}
