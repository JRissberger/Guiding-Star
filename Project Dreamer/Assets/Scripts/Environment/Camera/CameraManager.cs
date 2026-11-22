using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    GameObject player;

    [HideInInspector] new public CinemachineCamera camera;
    [HideInInspector] public CinemachineCamera memCamera;

    bool wasHiding;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        CinemachineCamera[] cam = GameObject.FindObjectsByType<CinemachineCamera>(FindObjectsSortMode.InstanceID);

        foreach (CinemachineCamera c in cam)
        {
            if (c.Priority == 2)
                camera = c;
            else if (c.Priority == 1)
                memCamera = c;
        }
        Debug.Log(camera);
        memCamera.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReturnFromMemory()
    {
        camera.gameObject.SetActive(true);
        memCamera.gameObject.SetActive(false);
        player.GetComponent<PlayerMovement>().canMove = true;
        if (!wasHiding)
            player.GetComponent<PlayerCorporeality>().SetHiding(false);
    }

    public void MemorySwitch(GameObject memory)
    {
        // Pause player movement, make them invulnerable to light if they are in it until after the memory is done
        player.GetComponent<PlayerMovement>().canMove = false;
        wasHiding = player.GetComponent<PlayerCorporeality>().IsHiding();
        if (!wasHiding)
            player.GetComponent<PlayerCorporeality>().SetHiding(true);

        // Align camera with current one so it can smoothly lerp to memory, then switch them
        memCamera.gameObject.SetActive(true);
        memCamera.ForceCameraPosition(camera.transform.position, camera.transform.rotation);
        camera.gameObject.SetActive(false);
        memCamera.Follow = memory.transform;
        Vector3 offset = (memory.transform.position - GameObject.FindGameObjectWithTag("Star").transform.position);
        offset.y += 2;
        offset = offset.normalized * 4;
        memCamera.GetComponent<CinemachineFollow>().FollowOffset = offset;
    }
}