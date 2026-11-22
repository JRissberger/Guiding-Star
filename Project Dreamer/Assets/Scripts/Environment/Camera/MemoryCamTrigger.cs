using Unity.Cinemachine;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Splines;
using Unity.VisualScripting;

public class MemoryCamTrigger : MonoBehaviour
{
    CameraManager manager;
    new CameraController camera;
    GameObject memory;

    void Start()
    {
        Collider[] overlaps = Physics.OverlapSphere(this.transform.position, this.transform.lossyScale.x);
        foreach (Collider col in overlaps)
        {
            if (col.gameObject.CompareTag("Memory"))
            {
                memory = col.gameObject;
                break;
            }
        }
        manager = GameObject.Find("CameraManager").GetComponent<CameraManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Star"))
        {
            GameObject.Find("CameraManager").GetComponent<CameraManager>().MemorySwitch(memory);
            Destroy(this);
        }
    }
}