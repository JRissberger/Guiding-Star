using System.Collections;
using UnityEngine;

/// <summary>
/// Event script to close the door and open the vent at the end of Room 3.
/// </summary>
public class CloseDoorOpenVent : MonoBehaviour
{
    [SerializeField] float doorShutTime = 0.5f;
    [SerializeField] float ventOpenTime = 1.0f;
    [SerializeField] GameObject doorInstance;
    [SerializeField] GameObject ventInstance;

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
            float angle = (1.0f - (doorTimer / doorShutTime)) * 90.0f;
            Vector3 doorRotation = doorInstance.transform.rotation.eulerAngles;
            doorRotation.y = angle;
            doorInstance.transform.rotation = Quaternion.Euler(doorRotation);
            yield return new WaitForSeconds(Time.deltaTime);
        }

        Vector3 finalDoorRotation = doorInstance.transform.rotation.eulerAngles;
        finalDoorRotation.y = 0.0f;
        doorInstance.transform.rotation = Quaternion.Euler(finalDoorRotation);

        float ventTimer = 0.0f;
        while (ventTimer < ventOpenTime)
        {
            ventTimer += Time.deltaTime;
            float angle = Mathf.Pow(ventTimer / ventOpenTime, 2.0f) * -60.0f;
            Vector3 ventRotation = ventInstance.transform.rotation.eulerAngles;
            ventRotation.z = angle;
            ventInstance.transform.rotation = Quaternion.Euler(ventRotation);
            yield return new WaitForSeconds(Time.deltaTime);
        }

        Vector3 finalVentRotation = ventInstance.transform.rotation.eulerAngles;
        finalVentRotation.z = -60.0f;
        ventInstance.transform.rotation = Quaternion.Euler(finalVentRotation);
    }
}
