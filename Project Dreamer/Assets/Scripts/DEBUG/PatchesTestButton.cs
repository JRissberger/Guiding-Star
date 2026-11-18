using System;
using UnityEngine;

/// <summary>
/// Used to test Patches functionality.
/// Unity doesn't let you use enums as parameters for OnClick, so this is a workaround.
/// </summary>
public class PatchesButtonTest : MonoBehaviour
{
    private Patches patches;

    private void Start()
    {
        patches = FindAnyObjectByType<Patches>();
    }

    public void DamagePart(int partIndex)
    {
        patches.SwapPart((Patches.PatchesBodyParts)partIndex, false);
    }

    public void RestorePart(int partIndex)
    {
        patches.SwapPart((Patches.PatchesBodyParts)partIndex, true);
    }

    public void CheckPart(int partIndex)
    {
        string name = Enum.GetNames(typeof(Patches.PatchesBodyParts))[partIndex];
        string status = patches.IsRestored((Patches.PatchesBodyParts)partIndex) ? "Restored" : "Damaged";
        Debug.Log("Part #" + partIndex + " (" + name + ") is: " + status);
    }
}
