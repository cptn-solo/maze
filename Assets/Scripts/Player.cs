using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private void Awake()
    {
        PlayerInput input = GetComponent<PlayerInput>();
        PlayerInputActions actions = new PlayerInputActions();
        actions.Enable();
        actions.Default.Jump.performed += Jump_performed;
        actions.Default.Move.performed += Move_performed;
    }

    private void Move_performed(InputAction.CallbackContext obj)
    {
        var dir = obj.ReadValue<Vector2>();
        Debug.Log($"Move {dir}");
    }

    private void Jump_performed(InputAction.CallbackContext obj)
    {
        Debug.Log($"Jump {obj}");
    }
}
