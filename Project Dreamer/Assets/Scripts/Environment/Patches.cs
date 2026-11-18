using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles switching out the model for each of Patches' body parts.
/// </summary>
public class Patches : MonoBehaviour
{
    public enum PatchesBodyParts
    {
        HEAD,
        BODY,
        LEFT_ARM,
        RIGHT_ARM,
        LEFT_LEG,
        RIGHT_LEG
    }

    [Serializable]
    public class PatchesSegment
    {
        public PatchesBodyParts partType;
        public GameObject damaged;
        public GameObject restored;
    }

    [SerializeField] private PatchesSegment[] segments;

    private void Start()
    {
        foreach (PatchesBodyParts part in Enum.GetValues(typeof(PatchesBodyParts)))
        {
            SwapPart(part, false);
        }
    }

    /// <summary>
    /// Sets a part of Patches to its restored or damaged appearance.
    /// </summary>
    /// <param name="partType">Which part of Patches to set.</param>
    /// <param name="restore">If true, the part will switch to its restored appearance; if false, it will switch to being damaged.</param>
    public void SwapPart(PatchesBodyParts partType, bool restore = true)
    {
        PatchesSegment part = GetSegmentByPart(partType);
        part.damaged.SetActive(!restore);
        part.restored.SetActive(restore);
    }

    /// <summary>
    /// Returns whether the given part of Patches is restored.
    /// </summary>
    /// <param name="partType">The part of Patches to check.</param>
    /// <returns>True if the part is restored, false if it is damaged.</returns>
    public bool IsRestored(PatchesBodyParts partType)
    {
        return GetSegmentByPart(partType).restored.activeInHierarchy;
    }

    /// <summary>
    /// Gets the PatchesSegment object associated with the given body part.
    /// A PatchesSegment has fields "damaged" (a GameObject of the damaged body part)
    ///  and "restored" (a GameObject of the restored body part).
    /// </summary>
    /// <param name="partType">Which part of Patches to get.</param>
    /// <returns>The corresponding PatchesSegment, or null if none is found.</returns>
    public PatchesSegment GetSegmentByPart(PatchesBodyParts partType)
    {
        foreach(PatchesSegment segment in segments)
        {
            if (segment.partType == partType)
            {
                return segment;
            }
        }
        return null;
    }
}
