using System.Collections;
using UnityEngine;

/// <summary>
/// Event script to close the door and open the vent at the end of Memory 1, Room 3.
/// </summary>
public class CloseDoorOpenVent : MonoBehaviour
{
    [SerializeField] private float doorShutTime = 0.5f;
    [SerializeField] private float ventOpenTime = 1.0f;
    [SerializeField] private GameObject doorInstance;
    [SerializeField] private GameObject ventInstance;

    private Vector3 doorStartRotation;
    private Vector3 ventStartRotation;

    public void Start()
    {
        doorStartRotation = doorInstance.transform.rotation.eulerAngles;
        ventStartRotation = ventInstance.transform.rotation.eulerAngles;
    }

    public void Fire()
    {
        StartCoroutine(EventCoroutine());
    }

    private IEnumerator EventCoroutine()
    {
        float doorTimer = 0.0f;

        while (doorTimer < doorShutTime)
        {
            doorTimer += Time.deltaTime;
            float angle = doorTimer / doorShutTime * -90.0f;
            Vector3 doorRotation = doorStartRotation;
            doorRotation.y += angle;
            doorInstance.transform.rotation = Quaternion.Euler(doorRotation);
            yield return new WaitForSeconds(Time.deltaTime);
        }

        Vector3 finalDoorRotation = doorStartRotation;
        finalDoorRotation.y -= 90.0f;
        doorInstance.transform.rotation = Quaternion.Euler(finalDoorRotation);

        float ventTimer = 0.0f;
        while (ventTimer < ventOpenTime)
        {
            ventTimer += Time.deltaTime;
            float angle = Mathf.Pow(ventTimer / ventOpenTime, 2.0f) * -60.0f;
            Vector3 ventRotation = ventStartRotation;
            ventRotation.z += angle;
            ventInstance.transform.rotation = Quaternion.Euler(ventRotation);
            yield return new WaitForSeconds(Time.deltaTime);
        }

        Vector3 finalVentRotation = ventStartRotation;
        finalVentRotation.z -= 60.0f;
        ventInstance.transform.rotation = Quaternion.Euler(finalVentRotation);
    }

    public void Reset()
    {
        doorInstance.transform.rotation = Quaternion.Euler(doorStartRotation);
        ventInstance.transform.rotation = Quaternion.Euler(ventStartRotation);
    }
}
