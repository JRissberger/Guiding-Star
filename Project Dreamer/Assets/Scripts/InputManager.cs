using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField] private InputActionAsset playerInput;

    private void Awake()
    {
        //Enable default map
        playerInput.FindActionMap("Movement").Enable();

    }
}
