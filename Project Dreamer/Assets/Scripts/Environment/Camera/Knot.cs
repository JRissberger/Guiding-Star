using System;
using UnityEngine;
[System.Serializable]

public class Knot : MonoBehaviour
{
    public Trigger trigger;
    public Transform lookAt;
    int knot;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        trigger = gameObject.GetComponentInChildren<Trigger>();
        lookAt = gameObject.GetComponentInChildren<SphereCollider>().transform;
        knot = int.Parse(this.name.Substring(5));
        trigger.knot = knot;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
