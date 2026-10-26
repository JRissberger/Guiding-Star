using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

public class TriggerManager : MonoBehaviour
{
    List<Trigger> triggers;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //triggers = GameObject.FindObjectsByType<Trigger>(FindObjectsSortMode.None);
        //triggers = triggers.OrderBy(o => o.OrderDate).ToList();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Time = (2 * force * (sin(Angle)))/ gravity
    
}
