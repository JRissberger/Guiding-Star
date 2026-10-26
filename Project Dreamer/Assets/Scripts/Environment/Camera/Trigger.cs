using Unity.Cinemachine;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Splines;
using Unity.VisualScripting;

public class Trigger : MonoBehaviour
{
    CameraController camera;
    [Tooltip("This should be equal to the camera knot that frames its volume, should be dynamically handled so long as it's under a Knot # in the hierarchy")]
    public int knot;
    [Tooltip("How long it will take to lerp to the next knot on the spline. -1 keeps it at the default (4s)")]
    [SerializeField]  float timeToMoveForward = -1; 
    [SerializeField] float timeToMoveBack= -1;
    public Vector3 forwardMovement = Vector3.zero;

    void Start()
    {
        if (forwardMovement == Vector3.zero)
            forwardMovement = this.transform.forward;

        camera = GameObject.Find("CinemachineCamera").GetComponent<CameraController>();
    }

    /*private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            if (camera.GetComponent<CinemachineSplineDolly>().CameraPosition != knot)
            {
                float time = timeToMoveForward;
                if (knot < camera.GetComponent<CinemachineSplineDolly>().CameraPosition) { time = timeToMoveBack; }
                camera.MoveTo(knot, time);
            }
        }
    }*/

    /// When the player leaves a trigger volume, move to the trigger knot that the player is in
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<PlayerMovement>(out PlayerMovement p))
        {
            List<Trigger> triggers = p.triggers;

            foreach (Trigger t in triggers)
            {
                if (t == this)
                    continue;

                float time = t.timeToMoveForward;
                if (t.knot < knot) { time = t.timeToMoveForward; }
                camera.MoveTo(t.knot, time);
                break;
            }
        }
    }
}