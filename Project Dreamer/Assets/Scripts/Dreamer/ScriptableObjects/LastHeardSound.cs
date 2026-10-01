using UnityEngine;

//Contains data for the last heard sound Star was tracking.
//Saved separately due to C# referencing removing target data when a sound despawns.
//Copying the sound directly would place another instance in the scene, which isn't ideal. So this is the solution.
[CreateAssetMenu(fileName = "LastHeardSound", menuName = "Scriptable Objects/LastHeardSound")]
public class LastHeardSound : ScriptableObject
{
    //Type of sound. Corresponds to Sound enum
    private SoundType soundType;
    public SoundType SoundType { get { return soundType; } set { soundType = value; } }

    //Location of the sound
    private Vector3 location;
    public Vector3 Location { get { return location; } set { location = value; } }

    //DEBUG--Confirming destruction
    private void OnDestroy()
    {
        Debug.Log("ScriptableObject destroyed.");
    }
}
