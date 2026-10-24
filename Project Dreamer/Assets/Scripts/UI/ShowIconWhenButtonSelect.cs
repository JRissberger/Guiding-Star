using Unity.AppUI.UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShowIconWhenButtonSelect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    [SerializeField] private GameObject Star;
    private Vector3 speed = new Vector3(0, 0, 25);
    public void Update()
    {
        Star.transform.Rotate(speed * Time.deltaTime);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Star.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Star.SetActive(false);
    }
}
