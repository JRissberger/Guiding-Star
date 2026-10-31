using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class LightSwitchInteractable : InteractableBase
{
    [Tooltip("The light(s) toggled by this switch")]
    [SerializeField] private List<LightHazard> lights;

    public override void Interact()
    {
        foreach(LightHazard light in lights)
        {
            light.ToggleLight();
        }
    }
}
