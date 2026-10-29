using UnityEngine;

// Attach this to cameras in scene, something is setting depth texture mode
// to none, and this is causing many effects that depend on reading from
// the depth texture to stop working.
[ExecuteAlways]
public class ForceDepth: MonoBehaviour
{
    void Start()
    {
        this.GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
    }
}
