using UnityEngine;

[ExecuteAlways]
public class LightHazardEditor : MonoBehaviour
{
    private Mesh _lightConeMesh;
    private Light _spotLight;
    private void Start()
    {
        _lightConeMesh = GetComponentInChildren<MeshFilter>().sharedMesh;
        _spotLight = GetComponentInChildren<Light>();
    }

    private void Update()
    {
        _spotLight.spotAngle = Mathf.Atan2(transform.lossyScale.x / 2, transform.lossyScale.y) * 2 * Mathf.Rad2Deg;
    }

    private void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireMesh(_lightConeMesh);
    }
}
